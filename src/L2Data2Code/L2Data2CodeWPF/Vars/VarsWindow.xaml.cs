using MahApps.Metro.Controls;

namespace L2Data2CodeWPF.Vars
{
    /// <summary>
    /// Interaction logic for VarsWindow.xaml
    /// </summary>
    public partial class VarsWindow : MetroWindow
    {
        public VarsWindow(VarsVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
