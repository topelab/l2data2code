using L2Data2Code.SharedContext.Main.TablePanel;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Base;

namespace L2Data2Code.SharedContext.Commands
{
    internal class SetDataItemsCommand : ReactiveBaseCommand, ISetDataItemsCommand
    {
        private TablePanelVM controlVM;

        public SetDataItemsCommand(ICommandManager commandManager) : base(commandManager)
        {
        }

        public void Initialize(TablePanelVM controlVM)
        {
            this.controlVM = controlVM;
        }

        public override void Execute(object parameter)
        {
            SetDataItems();
        }

        private void SetDataItems()
        {
            foreach (var item in controlVM.AllDataItems)
            {
                if (item.Value.IsSelected && item.Value.IsRelated)
                {
                    item.Value.IsSelected = false;
                }
            }
            foreach (var item in controlVM.AllDataItems)
            {
                if (item.Value.IsSelected)
                {
                    controlVM.SetDataItemCommand.Execute(item.Value);
                }
            }
        }
    }
}
