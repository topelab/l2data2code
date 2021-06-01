using L2Data2CodeWPF.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace L2Data2CodeWPF.Controls.TablePanel
{
    /// <summary>
    /// Interaction logic for TablePanelUserControl.xaml
    /// </summary>
    public partial class TablePanelUserControl : UserControl
    {
        private readonly TablePanelViewModel viewModel;
        private readonly MainWindowViewModel mainWindowViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="TablePanelUserControl"/> class.
        /// </summary>
        public TablePanelUserControl()
        {
            InitializeComponent();
            if (App.Current.MainWindow != null)
            {
                mainWindowViewModel = App.Current.MainWindow.DataContext as MainWindowViewModel;
                viewModel = mainWindowViewModel.TablePanelViewModel;
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
