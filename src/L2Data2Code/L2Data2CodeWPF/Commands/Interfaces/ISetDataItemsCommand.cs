using L2Data2CodeWPF.Controls.TablePanel;
using System.Windows.Input;

namespace L2Data2CodeWPF.Commands.Interfaces
{
    internal interface ISetDataItemsCommand : ICommand
    {
        void Initialize(TablePanelVM controlVM);
        void SetDataItems();
        void SetDataItem(TableVM tableVM);
        void AddToViews(TableVM element);
    }
}
