using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Main.TablePanel;

namespace L2Data2Code.SharedContext.Commands
{
    public class SetDataItemsCommand : DelegateCommandFactory, ISetDataItemsCommandFactory
    {
        private TablePanelVM controlVM;

        public void Initialize(TablePanelVM controlVM)
        {
            this.controlVM = controlVM;
        }

        protected override void Execute()
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
