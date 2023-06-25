using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedContext.Base;

namespace L2Data2Code.SharedContext.Main.TablePanel
{
    public class TableVM : ViewModelBase
    {
        readonly Table _table;
        private bool isSelected;
        private bool isRelated;
        private bool isVisible = true;

        public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }
        public bool IsRelated { get => isRelated; set => SetProperty(ref isRelated, value); }
        public bool IsVisible { get => isVisible; set => SetProperty(ref isVisible, value); }

        public string Name { get => _table.Name; }
        public Table Table => _table;

        public TableVM(Table table, bool isSelected = false, bool isRelated = false)
        {
            _table = table;
            IsSelected = isSelected;
            IsRelated = isRelated;
        }

    }
}
