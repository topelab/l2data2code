namespace L2Data2Code.SharedContext.Main.TablePanel
{
    internal interface ITablePanelChangeListener
    {
        void Start(MainWindowVM mainVM, TablePanelVM controlVM);
        void Stop();
    }
}