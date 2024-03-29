using L2Data2Code.SharedContext.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace L2Data2Code.SharedContext.Main.TablePanel
{
    /// <summary>
    /// View model for table panel
    /// </summary>
    public class TablePanelVM : ViewModelBase
    {
        private string _autoHeight = "50*";
        private bool _selectAllTables;
        private bool _loadingTables;
        private bool _selectAllViews;
        private bool viewsVisible = true;
        private bool setRelatedTables;
        private IDelegateCommand loadTablesCommand;
        private IDelegateCommand setDataItemsCommand;
        private IDelegateCommand setDataItemCommand;

        public ObservableCollection<TableVM> AllTables { get; set; } = [];
        public ObservableCollection<TableVM> AllViews { get; set; } = [];
        public Dictionary<string, TableVM> AllDataItems { get; set; } = [];

        public string AutoHeight
        {
            get => _autoHeight;
            set => SetProperty(ref _autoHeight, value);
        }

        public bool SelectAllTables
        {
            get => _selectAllTables;
            set => SetProperty(ref _selectAllTables, value, () => { SetTables(value); });
        }

        public bool LoadingTables
        {
            get => _loadingTables;
            set => SetProperty(ref _loadingTables, value);
        }

        public bool SelectAllViews
        {
            get => _selectAllViews;
            set => SetProperty(ref _selectAllViews, value, () => { SetViews(value); });
        }


        public bool ViewsVisible
        {
            get => viewsVisible;
            set => SetProperty(ref viewsVisible, value);
        }

        private bool isAnyItemActive;

        public bool IsAnyItemActive
        {
            get => isAnyItemActive;
            set => SetProperty(ref isAnyItemActive, value);
        }


        /// <summary>
        /// Indicate if has to set related tables
        /// </summary>
        public bool SetRelatedTables
        {
            get => setRelatedTables;
            internal set => SetProperty(ref setRelatedTables, value);
        }

        public IDelegateCommand LoadTablesCommand
        {
            get => loadTablesCommand;
            internal set => SetProperty(ref loadTablesCommand, value);
        }

        public IDelegateCommand SetDataItemsCommand
        {
            get => setDataItemsCommand;
            internal set => SetProperty(ref setDataItemsCommand, value);
        }

        public IDelegateCommand SetDataItemCommand
        {
            get => setDataItemCommand;
            internal set => SetProperty(ref setDataItemCommand, value);
        }

        public void SetCommands(IDelegateCommand loadTablesCommand, IDelegateCommand setDataItemsCommand, IDelegateCommand setDataItemCommand)
        {
            LoadTablesCommand = loadTablesCommand;
            SetDataItemsCommand = setDataItemsCommand;
            SetDataItemCommand = setDataItemCommand;
        }

        public void AddToViews(TableVM element)
        {
            var viewToAdd = !element.IsVisible ? null : element.Table.IsView ? AllViews : AllTables;
            viewToAdd?.Add(element);
            if (AllViews.Any())
            {
                OnPropertyChanged(nameof(AllViews));
            }
            else
            {
                OnPropertyChanged(nameof(AllTables));
            }
        }

        private void SetTables(bool selected)
        {
            var selection = (selected ? AllTables : AllDataItems.Values.Where(t => !t.Table.IsView)).ToList();
            foreach (var item in selection)
            {
                item.IsSelected = selected;
            }
        }

        private void SetViews(bool selected)
        {
            var selection = (selected ? AllViews : AllDataItems.Values.Where(t => t.Table.IsView)).ToList();
            foreach (var item in selection)
            {
                item.IsSelected = selected;
            }
        }
    }
}
