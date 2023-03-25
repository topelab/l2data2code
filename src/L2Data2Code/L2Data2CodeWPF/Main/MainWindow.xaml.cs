using L2Data2CodeWPF.Base;
using MahApps.Metro.Controls;
using System.Windows.Input;

namespace L2Data2CodeWPF.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        internal MainWindow(MainWindowVM mainWindowVM)
        {
            DataContext = mainWindowVM;
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
            => SharedViewEventManager.OnScrollViewerPreviewMouseWheel(sender, e);

    }
}