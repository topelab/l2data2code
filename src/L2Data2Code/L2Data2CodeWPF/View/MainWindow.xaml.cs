using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeWPF.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Unity;

namespace L2Data2CodeWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel viewModel;
        private Dispatcher dispatcher => Application.Current?.Dispatcher;

        public Timer CheckOpenedTimer { get; private set; }

        public MainWindow()
        {
            viewModel = SetupDI.Container.Resolve<MainWindowViewModel>();
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
                DragMove();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private FileSystemWatcher fsw;
        private void StartMonitorConfig()
        {
            var appBasePath = AppDomain.CurrentDomain.BaseDirectory.TrimPathSeparator();
            fsw = new FileSystemWatcher(appBasePath);
            fsw.NotifyFilter = NotifyFilters.LastWrite;
            var config = $"{appBasePath}\\appsettings.json";
            fsw.Changed += (s, e) => ReStartApplication(e, fsw, config);
            fsw.EnableRaisingEvents = true;

            CheckOpenedTimer = new Timer(viewModel.CheckOpenedTimerCallBack, null, 1000, 1000);
        }

        private void ReStartApplication(FileSystemEventArgs e, FileSystemWatcher fsw, string config)
        {
            if (e.FullPath.Equals(config, StringComparison.CurrentCultureIgnoreCase))
            {
                fsw.EnableRaisingEvents = false;
                dispatcher?.Invoke(() =>
                {
                    this.Activate();
                    var result = MessageBox.Show(this, Strings.ConfigChanged, Strings.Warning, MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Cancel)
                    {
                        fsw.EnableRaisingEvents = true;
                        return;
                    }
                    App.RestartApp = true;
                    this.Close();
                });
            }
        }

        private void ExpanderInfoMessages_Expanded(object sender, RoutedEventArgs e)
        {
            if (viewModel == null) return;

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