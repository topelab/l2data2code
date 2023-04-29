using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Localize;
using L2Data2Code.Base;
using System;
using System.Threading;
using System.Windows;
using L2Data2Code.Main.Interfaces;

namespace L2Data2Code.Main
{
    internal class MainWindowEventManager : IMainWindowEventManager
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IFileMonitorService fileMonitorService;
        private readonly IProcessManager processManager;
        private readonly IGeneratorAdapter generatorAdapter;

        public Timer CheckOpenedTimer { get; private set; }

        public MainWindowEventManager(IDispatcherWrapper dispatcherWrapper, IFileMonitorService fileMonitorService, IProcessManager processManager, IGeneratorAdapter generatorAdapter)
        {
            this.dispatcherWrapper = dispatcherWrapper ?? throw new ArgumentNullException(nameof(dispatcherWrapper));
            this.fileMonitorService = fileMonitorService ?? throw new ArgumentNullException(nameof(fileMonitorService));
            this.processManager = processManager ?? throw new ArgumentNullException(nameof(processManager));
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
        }

        public void Start(MainWindow window, MainWindowVM mainWindowVM)
        {
            var appBasePath = AppDomain.CurrentDomain.BaseDirectory.TrimPathSeparator();
            fileMonitorService.StartMonitoring((file) => ReStartApplication(window, file), appBasePath, BasicNameValueConfiguration.APP_SETTINGS_FILE);
            CheckOpenedTimer = new Timer((state) => CheckOpenedTimerCallBack(mainWindowVM), null, 3000, 3000);
        }

        private void ReStartApplication(MainWindow window, string fileChanged)
        {
            if (fileChanged.Equals(BasicNameValueConfiguration.APP_SETTINGS_FILE, StringComparison.CurrentCultureIgnoreCase))
            {
                dispatcherWrapper?.Invoke(() =>
                {
                    window.Activate();
                    var result = MessageBox.Show(Strings.ConfigChanged, Strings.Warning, MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    App.RestartApp = true;
                    window.Close();
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
                App.Logger.Error($"{nameof(MainWindowVM)}.{nameof(CheckOpenedTimerCallBack)}(): {ex.Message}");
            }

            mainWindowVM.PauseTimer = false;
        }



    }
}
