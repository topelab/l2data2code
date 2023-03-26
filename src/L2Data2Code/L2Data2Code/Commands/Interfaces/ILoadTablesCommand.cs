using L2Data2Code.Main;
using L2Data2Code.Main.TablePanel;
using System.Windows.Input;

namespace L2Data2Code.Commands.Interfaces
{
    internal interface ILoadTablesCommand : ICommand
    {
        void Initialize(MainWindowVM mainVM, TablePanelVM controlVM);
    }
}
