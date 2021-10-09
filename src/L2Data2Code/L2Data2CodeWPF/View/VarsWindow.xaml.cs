using L2Data2CodeWPF.ViewModel;
using MahApps.Metro.Controls;

namespace L2Data2CodeWPF
{
    /// <summary>
    /// Interaction logic for VarsWindow.xaml
    /// </summary>
    public partial class VarsWindow : MetroWindow
    {
        public VarsWindow(VarsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
