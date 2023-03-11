using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.Commands.Interfaces;
using L2Data2CodeWPF.Controls.TablePanel;
using System.Linq;

namespace L2Data2CodeWPF.Commands
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

        public void SetDataItems()
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
                    SetDataItem(item.Value);
                }
            }
        }

        public void SetDataItem(TableVM tableVM)
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
                            AddToViews(tableToSee);
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

        public void AddToViews(TableVM element)
        {
            var viewToAdd = !element.IsVisible ? null : (element.Table.IsView) ? controlVM.AllViews : controlVM.AllTables;
            viewToAdd?.Add(element);
        }


    }
}
