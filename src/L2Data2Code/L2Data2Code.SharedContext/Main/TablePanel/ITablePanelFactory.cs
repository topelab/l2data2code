namespace L2Data2Code.SharedContext.Main.TablePanel
{
    internal interface ITablePanelFactory
    {
        TablePanelVM Create(MainWindowVM mainVM);
    }
}