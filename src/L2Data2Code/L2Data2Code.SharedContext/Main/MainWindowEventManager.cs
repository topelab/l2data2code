using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Events;
using L2Data2Code.SharedContext.Main.Interfaces;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Localize;
using NLog;
using Prism.Events;
using System;
using System.Threading;

namespace L2Data2Code.SharedContext.Main
{
    internal class MainWindowEventManager : IMainWindowEventManager
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IFileMonitorService fileMonitorService;
        private readonly IProcessManager processManager;
        private readonly IGeneratorAdapter generatorAdapter;
        private readonly ILogger logger;
        private readonly IEventAggregator eventAggregator;
        private readonly IMessageBoxWrapper messageBox;

        public Timer CheckOpenedTimer { get; private set; }

        public MainWindowEventManager(IDispatcherWrapper dispatcherWrapper, IFileMonitorService fileMonitorService, IProcessManager processManager, IGeneratorAdapter generatorAdapter, ILogger logger, IEventAggregator eventAggregator, IMessageBoxWrapper messageBox)
        {
            this.dispatcherWrapper = dispatcherWrapper ?? throw new ArgumentNullException(nameof(dispatcherWrapper));
            this.fileMonitorService = fileMonitorService ?? throw new ArgumentNullException(nameof(fileMonitorService));
            this.processManager = processManager ?? throw new ArgumentNullException(nameof(processManager));
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.messageBox = messageBox ?? throw new ArgumentNullException(nameof(messageBox));
        }

        public void Start(ISimpleWindow window, MainWindowVM mainWindowVM)
        {
            var appBasePath = AppDomain.CurrentDomain.BaseDirectory.TrimPathSeparator();
            fileMonitorService.StartMonitoring((file) => ReStartApplication(window, file), appBasePath, BasicNameValueConfiguration.APP_SETTINGS_FILE);
            CheckOpenedTimer = new Timer((state) => CheckOpenedTimerCallBack(mainWindowVM), null, 3000, 3000);
        }

        private void ReStartApplication(ISimpleWindow window, string fileChanged)
        {
            if (fileChanged.Equals(BasicNameValueConfiguration.APP_SETTINGS_FILE, StringComparison.CurrentCultureIgnoreCase))
            {
                dispatcherWrapper?.Invoke(() =>
                {
                    eventAggregator.GetEvent<SimpleWindowEvent>().Publish(new SimpleWindowEventArgs { Target = nameof(MainWindowVM), Action = SimpleWindowEventActions.Activate });
                    var result = messageBox.Show(Strings.ConfigChanged, Strings.Warning, MessageBoxWrapperButton.OKCancel, MessageBoxWrapperImage.Question);
                    if (result == MessageBoxWrapperResult.Cancel)
                    {
                        return;
                    }
                    eventAggregator.GetEvent<SimpleWindowEvent>().Publish(new SimpleWindowEventArgs { Target = nameof(MainWindowVM), Action = SimpleWindowEventActions.Restart });
                });
            }
        }

        public async void CheckOpenedTimerCallBack(MainWindowVM mainWindowVM)
        {
            if (mainWindowVM.PauseTimer)
            {
                return;
            }
            mainWindowVM.PauseTimer = true;

            try
            {
                await processManager.UpdateRunningEditors();
                await processManager.CheckSolutionOpened();
                await processManager.CheckEditorsOpenedAsync(generatorAdapter.SettingsConfiguration["Editor"]);
            }
            catch (Exception ex)
            {
                logger.Error($"{nameof(MainWindowVM)}.{nameof(CheckOpenedTimerCallBack)}(): {ex.Message}");
            }

            mainWindowVM.PauseTimer = false;
        }



    }
}
