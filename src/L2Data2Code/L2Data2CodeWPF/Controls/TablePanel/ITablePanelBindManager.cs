using L2Data2CodeWPF.Main;

namespace L2Data2CodeWPF.Controls.TablePanel
{
    internal interface ITablePanelBindManager
    {
        void Start(MainWindowVM mainVM, TablePanelVM controlVM);
        void Stop();
    }
}