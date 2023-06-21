using L2Data2Code.MAUI.Base;
using L2Data2Code.MAUI.Main.TablePanel;
using L2Data2Code.MAUI.Commands.Interfaces;

namespace L2Data2Code.MAUI.Commands
{
    internal class SetDataItemsCommand : DelegateCommand, ISetDataItemsCommand
    {
        private TablePanelVM controlVM;

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
