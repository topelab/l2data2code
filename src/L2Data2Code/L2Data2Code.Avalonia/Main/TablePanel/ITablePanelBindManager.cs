using L2Data2Code.Main;

namespace L2Data2Code.Main.TablePanel
{
    internal interface ITablePanelBindManager
    {
        void Start(MainWindowVM mainVM, TablePanelVM controlVM);
        void Stop();
    }
}