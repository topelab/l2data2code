using L2Data2Code.Base;
using L2Data2Code.Main;
using System.Windows.Controls;
using System.Windows.Input;

namespace L2Data2Code.Main.TablePanel
{
    /// <summary>
    /// Interaction logic for TablePanelUserControl.xaml
    /// </summary>
    public partial class TablePanelUserControl : UserControl
    {
        private readonly TablePanelVM viewModel;
        private readonly MainWindowVM mainWindowVM;

        /// <summary>
        /// Initializes a new instance of the <see cref="TablePanelUserControl"/> class.
        /// </summary>
        public TablePanelUserControl()
        {
            InitializeComponent();
            if (App.Current.MainWindow != null)
            {
                mainWindowVM = App.Current.MainWindow.DataContext as MainWindowVM;
                viewModel = mainWindowVM.TablePanelVM;
                DataContext = viewModel;
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
            => SharedViewEventManager.OnScrollViewerPreviewMouseWheel(sender, e);


    }
}
