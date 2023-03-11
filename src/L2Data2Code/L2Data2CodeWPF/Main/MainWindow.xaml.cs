using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Localize;
using L2Data2CodeWPF.SharedLib;
using MahApps.Metro.Controls;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace L2Data2CodeWPF.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowVM viewModel;
        private readonly IFileMonitorService fileMonitorService;
        private readonly IDispatcherWrapper dispatcher;

        public Timer CheckOpenedTimer { get; private set; }

        public MainWindow()
        {
            viewModel = App.Resolver.Get<MainWindowVM>();
            fileMonitorService = App.Resolver.Get<IFileMonitorService>();
            dispatcher = App.Resolver.Get<IDispatcherWrapper>();
            DataContext = viewModel;
            InitializeComponent();
            StartMonitorConfig();
            Title = $"{Strings.Title} v{viewModel.GeneratorVersion}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!viewModel.Working)
            {
                Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void StartMonitorConfig()
        {
            var appBasePath = AppDomain.CurrentDomain.BaseDirectory.TrimPathSeparator();
            fileMonitorService.StartMonitoring(ReStartApplication, appBasePath, AppSettingsConfiguration.APP_SETTINGS_FILE);
            CheckOpenedTimer = new Timer(viewModel.CheckOpenedTimerCallBack, null, 1000, 1000);
        }

        private void ReStartApplication(string fileChanged)
        {
            if (fileChanged.Equals(AppSettingsConfiguration.APP_SETTINGS_FILE, StringComparison.CurrentCultureIgnoreCase))
            {
                dispatcher?.Invoke(() =>
                {
                    Activate();
                    var result = MessageBox.Show(this, Strings.ConfigChanged, Strings.Warning, MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    App.RestartApp = true;
                    Close();
                });
            }
        }

        private void ExpanderInfoMessages_Expanded(object sender, RoutedEventArgs e)
        {
            if (viewModel == null)
            {
                return;
            }

            var expander = (Expander)sender;
            if (expander.IsExpanded)
            {
                expander.HorizontalAlignment = HorizontalAlignment.Stretch;
                viewModel.MessagePanelOpened = true;
            }
            else
            {
                expander.HorizontalAlignment = HorizontalAlignment.Left;
                viewModel.MessagePanelOpened = false;
            }
        }
    }
}