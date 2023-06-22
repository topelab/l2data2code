namespace L2Data2Code.MAUI.Main.TablePanel
{
    internal interface ITablePanelBindManager
    {
        void Start(MainWindowVM mainVM, TablePanelVM controlVM);
        void Stop();
    }
}