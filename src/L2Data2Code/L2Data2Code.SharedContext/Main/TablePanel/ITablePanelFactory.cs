namespace L2Data2Code.SharedContext.Main.TablePanel
{
    public interface ITablePanelFactory
    {
        TablePanelVM Create(MainWindowVM mainVM);
    }
}