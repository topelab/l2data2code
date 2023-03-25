using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.Controls.TablePanel;
using L2Data2CodeWPF.Main;
using System.Windows.Input;

namespace L2Data2CodeWPF.Commands.Interfaces
{
    internal interface ILoadTablesCommand : ICommand
    {
        void Initialize(MainWindowVM mainVM, TablePanelVM controlVM);
    }
}
