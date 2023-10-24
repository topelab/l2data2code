using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Main.TablePanel;

namespace L2Data2Code.SharedContext.Commands.Interfaces
{
    public interface ISetDataItemsCommandFactory : IDelegateCommandFactory
    {
        void Initialize(TablePanelVM controlVM);
    }
}
