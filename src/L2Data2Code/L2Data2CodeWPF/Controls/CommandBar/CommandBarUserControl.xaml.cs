using L2Data2CodeUI.Shared.Localize;
using L2Data2CodeWPF.Main;
using L2Data2CodeWPF.SharedLib;
using MahApps.Metro.IconPacks;
using System.Windows.Controls;

namespace L2Data2CodeWPF.Controls.CommandBar
{
    /// <summary>
    /// Interaction logic for CommandBarUserControl.xaml
    /// </summary>
    public partial class CommandBarUserControl : UserControl
    {
        private readonly CommandBarVM commandBarVM;
        private readonly MainWindowVM mainWindowVM;
        private readonly IDispatcherWrapper dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBarUserControl"/> class.
        /// </summary>
        public CommandBarUserControl()
        {
            InitializeComponent();
            if (App.Current.MainWindow != null)
            {
                mainWindowVM = App.Current.MainWindow.DataContext as MainWindowVM;
                dispatcher = mainWindowVM.Dispatcher;
                commandBarVM = mainWindowVM.CommandBarVM;
                DataContext = commandBarVM;
                commandBarVM.PropertyChanged += OnCommandBarVMPropertyChanged;
                this.Unloaded += OnCommandBarUserControlUnloaded;
            }
        }

        private void OnCommandBarUserControlUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Unloaded -= OnCommandBarUserControlUnloaded;
            if (commandBarVM != null)
            {
                commandBarVM.PropertyChanged -= OnCommandBarVMPropertyChanged;
            }
        }

        private void OnCommandBarVMPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(commandBarVM.ChangeButtons))
            {
                dispatcher.Invoke(() =>
                {
                    (OpenCmdButton.Content as PackIconSimpleIcons).Kind = mainWindowVM.AppType switch
                    {
                        L2Data2CodeUI.Shared.Dto.AppType.VisualStudio => PackIconSimpleIconsKind.VisualStudio,
                        L2Data2CodeUI.Shared.Dto.AppType.VisualStudioCode => PackIconSimpleIconsKind.VisualStudioCode,
                        L2Data2CodeUI.Shared.Dto.AppType.ApacheNetBeans => PackIconSimpleIconsKind.ApacheNetBeansIde,
                        L2Data2CodeUI.Shared.Dto.AppType.Eclipse => PackIconSimpleIconsKind.EclipseIde,
                        L2Data2CodeUI.Shared.Dto.AppType.IntelliJIdea => PackIconSimpleIconsKind.IntelliJIdea,
                        _ => PackIconSimpleIconsKind.None
                    };

                    OpenCmdButton.ToolTip = mainWindowVM.AppType switch
                    {
                        L2Data2CodeUI.Shared.Dto.AppType.VisualStudio => Strings.OpenVSSolution,
                        L2Data2CodeUI.Shared.Dto.AppType.VisualStudioCode => Strings.OpenVSC,
                        L2Data2CodeUI.Shared.Dto.AppType.ApacheNetBeans => Strings.Open + " Apache NetBeans",
                        L2Data2CodeUI.Shared.Dto.AppType.Eclipse => Strings.Open + " Eclipse",
                        L2Data2CodeUI.Shared.Dto.AppType.IntelliJIdea => Strings.Open + " IntelliJIdea",
                        _ => string.Empty
                    };
                });
            }
        }
    }
}
