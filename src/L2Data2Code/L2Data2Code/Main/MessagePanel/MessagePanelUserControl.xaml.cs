using L2Data2Code.Base;
using L2Data2Code.Main;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace L2Data2Code.Main.MessagePanel
{
    /// <summary>
    /// Interaction logic for MessagePanelUserControl.xaml
    /// </summary>
    public partial class MessagePanelUserControl : UserControl
    {
        private readonly MessagePanelVM messagePanelVM;
        private readonly MainWindowVM mainWindowVM;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePanelUserControl"/> class.
        /// </summary>
        public MessagePanelUserControl()
        {
            InitializeComponent();
            if (App.Current.MainWindow != null)
            {
                mainWindowVM = App.Current.MainWindow.DataContext as MainWindowVM;
                messagePanelVM = mainWindowVM.MessagePanelVM;
                DataContext = messagePanelVM;
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
            => SharedViewEventManager.OnScrollViewerPreviewMouseWheel(sender, e);

    }
}
