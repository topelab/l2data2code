using L2Data2Code.Main;
using System.Windows.Controls;

namespace L2Data2Code.Main.CommandBar
{
    /// <summary>
    /// Interaction logic for CommandBarUserControl.xaml
    /// </summary>
    public partial class CommandBarUserControl : UserControl
    {
        private readonly CommandBarVM commandBarVM;
        private readonly MainWindowVM mainWindowVM;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBarUserControl"/> class.
        /// </summary>
        public CommandBarUserControl()
        {
            InitializeComponent();
            if (App.Current.MainWindow != null)
            {
                mainWindowVM = App.Current.MainWindow.DataContext as MainWindowVM;
                commandBarVM = mainWindowVM.CommandBarVM;
                DataContext = commandBarVM;
            }
        }
    }
}
