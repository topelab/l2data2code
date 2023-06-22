using L2Data2Code.MAUI.Main.TablePanel;
using System.Windows.Input;

namespace L2Data2Code.MAUI.Commands.Interfaces
{
    internal interface ILoadTablesCommand : ICommand
    {
        void Initialize(TablePanelVM controlVM);
    }
}
