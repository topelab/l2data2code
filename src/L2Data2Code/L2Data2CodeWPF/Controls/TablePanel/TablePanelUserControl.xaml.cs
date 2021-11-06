using L2Data2CodeWPF.Main;
using System.Windows.Controls;
using System.Windows.Input;

namespace L2Data2CodeWPF.Controls.TablePanel
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
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }


    }
}
