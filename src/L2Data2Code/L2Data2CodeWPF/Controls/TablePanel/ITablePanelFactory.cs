using L2Data2CodeWPF.Main;

namespace L2Data2CodeWPF.Controls.TablePanel
{
    internal interface ITablePanelFactory
    {
        TablePanelVM Create(MainWindowVM mainVM);
    }
}