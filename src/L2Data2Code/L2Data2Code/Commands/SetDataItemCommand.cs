using L2Data2Code.Commands.Interfaces;
using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.Controls.TablePanel;
using System.Linq;

namespace L2Data2Code.Commands
{
    internal class SetDataItemCommand : DelegateCommand, ISetDataItemCommand
    {
        private TablePanelVM controlVM;

        public void Initialize(TablePanelVM controlVM)
        {
            this.controlVM = controlVM;
        }

        public override bool CanExecute(object parameter)
        {
            return parameter is TableVM;
        }

        public override void Execute(object parameter)
        {
            var tableVM = (TableVM)parameter;
            SetDataItem(tableVM);
        }

        private void SetDataItem(TableVM tableVM)
        {
            if (!controlVM.SetRelatedTables)
            {
                return;
            }

            var item = tableVM.Table;
            var selected = tableVM.IsSelected;
            var references = item.OuterKeys.Select(r => r.ColumnReferenced.Table)
                .Union(item.InnerKeys.Select(r => r.ColumnReferencing.Table))
                .Distinct();

            foreach (var table in references)
            {
                var tableToSee = controlVM.AllDataItems[table.Name];
                if (selected)
                {
                    if (!tableToSee.IsSelected)
                    {
                        tableToSee.IsRelated = true;
                        tableToSee.IsSelected = true;
                        if (!tableToSee.IsVisible)
                        {
                            tableToSee.IsVisible = true;
                            controlVM.AddToViews(tableToSee);
                        }
                    }
                }
                else
                {
                    if (tableToSee.IsSelected)
                    {
                        tableToSee.IsSelected = false;
                    }
                }
            }
        }
    }
}
