using L2Data2CodeWPF.ViewModel;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
