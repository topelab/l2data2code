using L2Data2Code.SharedContext.Main.TablePanel;
using System.Windows.Input;

namespace L2Data2Code.SharedContext.Commands.Interfaces
{
    public interface ISetDataItemsCommand : ICommand
    {
        void Initialize(TablePanelVM controlVM);
    }
}
