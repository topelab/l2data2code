using L2Data2Code.Main;

namespace L2Data2Code.Main.TablePanel
{
    internal interface ITablePanelFactory
    {
        TablePanelVM Create(MainWindowVM mainVM);
    }
}