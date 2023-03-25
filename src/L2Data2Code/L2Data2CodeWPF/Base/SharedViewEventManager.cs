using System.Windows.Controls;
using System.Windows.Input;

namespace L2Data2CodeWPF.Base
{
    internal class SharedViewEventManager
    {
        public static void OnScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

    }
}
