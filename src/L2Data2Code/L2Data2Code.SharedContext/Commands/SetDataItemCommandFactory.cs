using System.Linq;
using L2Data2Code.SharedContext.Main.TablePanel;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Base;

namespace L2Data2Code.SharedContext.Commands
{
    public class SetDataItemCommandFactory : DelegateCommandFactory<TableVM>, ISetDataItemCommandFactory
    {
        private TablePanelVM controlVM;

        public void Initialize(TablePanelVM controlVM)
        {
            this.controlVM = controlVM;
        }

        protected override void Execute(TableVM tableVM)
        {
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
