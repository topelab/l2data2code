using L2Data2Code.MAUI;
using L2Data2Code.MAUI.Base;
using L2Data2Code.MAUI.Main.Interfaces;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Localize;
using System.Windows;

namespace L2Data2Code.MAUI.Main
{
    internal class MainPageEventManager : IMainPageEventManager
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IFileMonitorService fileMonitorService;
        private readonly IProcessManager processManager;
        private readonly IGeneratorAdapter generatorAdapter;

        public Timer CheckOpenedTimer { get; private set; }

        public MainPageEventManager(IDispatcherWrapper dispatcherWrapper, IFileMonitorService fileMonitorService, IProcessManager processManager, IGeneratorAdapter generatorAdapter)
        {
            this.dispatcherWrapper = dispatcherWrapper ?? throw new ArgumentNullException(nameof(dispatcherWrapper));
            this.fileMonitorService = fileMonitorService ?? throw new ArgumentNullException(nameof(fileMonitorService));
            this.processManager = processManager ?? throw new ArgumentNullException(nameof(processManager));
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
        }

        public void Start(Page window, MainPageVM mainWindowVM)
        {
            var appBasePath = AppDomain.CurrentDomain.BaseDirectory.TrimPathSeparator();
            fileMonitorService.StartMonitoring((file) => ReStartApplication(window, file), appBasePath, BasicNameValueConfiguration.APP_SETTINGS_FILE);
            CheckOpenedTimer = new Timer((state) => CheckOpenedTimerCallBack(mainWindowVM), null, 3000, 3000);
        }

        private void ReStartApplication(Page window, string fileChanged)
        {
            if (fileChanged.Equals(BasicNameValueConfiguration.APP_SETTINGS_FILE, StringComparison.CurrentCultureIgnoreCase))
            {
                dispatcherWrapper?.Invoke(() =>
                {
                    window.Focus();
                    var result = MessageBox.Show(Strings.ConfigChanged, Strings.Warning, MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    App.RestartApp = true;
                    App.Current.CloseWindow(App.Current.Windows.First());
                });
            }
        }

        public async void CheckOpenedTimerCallBack(MainPageVM mainWindowVM)
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
                App.Logger.Error($"{nameof(MainPageVM)}.{nameof(CheckOpenedTimerCallBack)}(): {ex.Message}");
            }

            mainWindowVM.PauseTimer = false;
        }



    }
}
