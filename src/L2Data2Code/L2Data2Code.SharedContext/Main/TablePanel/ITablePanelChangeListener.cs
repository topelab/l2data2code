namespace L2Data2Code.SharedContext.Main.TablePanel
{
    public interface ITablePanelChangeListener
    {
        void Start(MainWindowVM mainVM, TablePanelVM controlVM);
        void Stop();
    }
}