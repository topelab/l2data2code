using L2Data2CodeWPF.Controls.TablePanel;
using System.Windows.Input;

namespace L2Data2Code.Commands.Interfaces
{
    internal interface ISetDataItemCommand : ICommand
    {
        void Initialize(TablePanelVM controlVM);
    }
}
