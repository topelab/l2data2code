using L2Data2CodeWPF.ViewModel;
using MahApps.Metro.IconPacks;
using System.Windows.Controls;

namespace L2Data2CodeWPF.Controls.CommandBar
{
    /// <summary>
    /// Interaction logic for CommandBarUserControl.xaml
    /// </summary>
    public partial class CommandBarUserControl : UserControl
    {
        private readonly CommandBarViewModel viewModel;
        private readonly MainWindowViewModel mainWindowViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBarUserControl"/> class.
        /// </summary>
        public CommandBarUserControl()
        {
            InitializeComponent();
            if (App.Current.MainWindow != null)
            {
                mainWindowViewModel = App.Current.MainWindow.DataContext as MainWindowViewModel;
                viewModel = mainWindowViewModel.CommandBarViewModel;
                DataContext = viewModel;
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.ChangeButtons))
            {
                (OpenCmdButton.Content as PackIconSimpleIcons).Kind = mainWindowViewModel.AppType switch
                {
                    L2Data2CodeUI.Shared.Dto.AppType.VisualStudio => PackIconSimpleIconsKind.VisualStudio,
                    L2Data2CodeUI.Shared.Dto.AppType.VisualStudioCode => PackIconSimpleIconsKind.VisualStudioCode,
                    L2Data2CodeUI.Shared.Dto.AppType.ApacheNetBeans => PackIconSimpleIconsKind.ApacheNetBeansIde,
                    L2Data2CodeUI.Shared.Dto.AppType.Eclipse => PackIconSimpleIconsKind.EclipseIde,
                    L2Data2CodeUI.Shared.Dto.AppType.IntelliJIdea => PackIconSimpleIconsKind.IntelliJIdea,
                    _ => PackIconSimpleIconsKind.None
                };

                OpenCmdButton.ToolTip = mainWindowViewModel.AppType switch
                {
                    L2Data2CodeUI.Shared.Dto.AppType.VisualStudio => Strings.OpenVSSolution,
                    L2Data2CodeUI.Shared.Dto.AppType.VisualStudioCode => Strings.OpenVSC,
                    L2Data2CodeUI.Shared.Dto.AppType.ApacheNetBeans => Strings.Open + " Apache NetBeans",
                    L2Data2CodeUI.Shared.Dto.AppType.Eclipse => Strings.Open + " Eclipse",
                    L2Data2CodeUI.Shared.Dto.AppType.IntelliJIdea => Strings.Open + " IntelliJIdea",
                    _ => string.Empty
                };
            }
        }
    }
}
