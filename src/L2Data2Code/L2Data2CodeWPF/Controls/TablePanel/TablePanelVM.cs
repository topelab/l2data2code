using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.Main;
using L2Data2CodeWPF.SharedLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace L2Data2CodeWPF.Controls.TablePanel
{
    /// <summary>
    /// View model for table panel
    /// </summary>
    /// <seealso cref="L2Data2CodeWPF.Base.BaseControlVM" />
    public class TablePanelVM : BaseControlVM
    {
        private readonly IGeneratorAdapter adapter;
        private readonly MainWindowVM mainWindowVM;
        private readonly IDispatcherWrapper dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="TablePanelVM"/> class.
        /// </summary>
        /// <param name="baseVM">The base view model.</param>
        public TablePanelVM(IBaseVM baseVM,
                                   IDispatcherWrapper dispatcher) : base(baseVM)
        {
            mainWindowVM = (MainWindowVM)baseVM;
            adapter = mainWindowVM.Adapter;
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public ObservableCollection<TableVM> AllTables { get; set; } = new ObservableCollection<TableVM>();
        public ObservableCollection<TableVM> AllViews { get; set; } = new ObservableCollection<TableVM>();
        public Dictionary<string, TableVM> AllDataItems { get; set; } = new Dictionary<string, TableVM>();

        private string _autoHeight = "50*";
        public string AutoHeight
        {
            get { return _autoHeight; }
            set { SetProperty(ref _autoHeight, value); }
        }

        private bool _selectAllTables;
        public bool SelectAllTables
        {
            get { return _selectAllTables; }
            set { SetProperty(ref _selectAllTables, value, () => { SetTables(value); }); }
        }

        private bool _loadingTables;
        public bool LoadingTables
        {
            get { return _loadingTables; }
            set { SetProperty(ref _loadingTables, value); }
        }

        private bool _selectAllViews;
        public bool SelectAllViews
        {
            get { return _selectAllViews; }
            set { SetProperty(ref _selectAllViews, value, () => { SetViews(value); }); }
        }

        private bool viewsVisible = true;
        public bool ViewsVisible
        {
            get { return viewsVisible; }
            set { SetProperty(ref viewsVisible, value, () => AutoHeight = value ? "50*" : "*"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [set related tables].
        /// </summary>
        /// <remarks>Needs to be subcribed to changes on <see cref="MainWindowVM.SetRelatedTables"/></remarks>
        /// <value>
        ///   <c>true</c> if [set related tables]; otherwise, <c>false</c>.
        /// </value>
        public bool SetRelatedTables
        {
            get { return mainWindowVM.SetRelatedTables; }
            set { mainWindowVM.SetRelatedTables = value; }
        }

        /// <summary>
        /// Includes / exclude tables changed.
        /// </summary>
        public void IncludeExcludeTablesChanged()
        {
            Working = true;
            LoadingTables = true;
            ViewsVisible = false;
            var includeTables = adapter.ModulesConfiguration[mainWindowVM.SelectedModule].IncludeTables;
            var excludeTables = adapter.ModulesConfiguration[mainWindowVM.SelectedModule].ExcludeTables;
            var includeTablesRegex = includeTables == null ? null : new Regex(includeTables);
            var excludeTablesRegex = excludeTables == null ? null : new Regex(excludeTables);

            Task.Run(() =>
            {
                mainWindowVM.PauseTimer = true;
                PopulateDataItems(includeTablesRegex, excludeTablesRegex);
            }).ContinueWith((t) =>
            {
                dispatcher.Invoke(() =>
                {
                    LoadingTables = false;
                    Working = false;
                    mainWindowVM.PauseTimer = false;
                });
            });
        }

        private void ClearDataItemsLists()
        {
            AllTables.Clear();
            AllViews.Clear();

            SelectAllTables = false;
            SelectAllViews = false;
        }

        /// <summary>
        /// Loads all tables.
        /// </summary>
        public void LoadAllTables()
        {
            App.Logger.Info("Loading data base items");
            AllDataItems.Clear();

            foreach (var item in adapter.Tables.OrderBy(k => k.Key))
            {
                TableVM element = new(item.Value);
                element.PropertyChanged += TableVM_PropertyChanged;
                AllDataItems.Add(element.Name, element);
            }
            App.Logger.Info("All data base items loaded");
        }

        private void PopulateDataItems(Regex includeTablesRegex = null, Regex excludeTablesRegex = null)
        {
            App.Logger.Info("Populating tables and view lists");
            dispatcher.Invoke(ClearDataItemsLists);

            foreach (var element in AllDataItems.Values.OrderBy(k => k.Name))
            {
                element.IsVisible = includeTablesRegex == null || includeTablesRegex.IsMatch(element.Name);
                if (excludeTablesRegex != null && element.IsVisible)
                {
                    element.IsVisible = !excludeTablesRegex.IsMatch(element.Name);
                }
                dispatcher.Invoke(AddToViews, element);
            }
            dispatcher.Invoke(() => ViewsVisible = AllViews.Any());
            App.Logger.Info("All items populated");
        }

        private void TableVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TableVM.IsSelected))
            {
                TableVM item = (TableVM)sender;
                if (item.IsSelected)
                {
                    App.Logger.Trace($"*** Table: {item.Name} has been selected");
                    SetDataItem(item);
                }
                else
                {
                    App.Logger.Trace($"*** Table: {item.Name} has unselected");
                    item.IsRelated = false;
                }

                mainWindowVM.OnPropertyChanged(nameof(mainWindowVM.GenerateCodeCommand));
            }
        }

        private void SetTables(bool selected)
        {
            List<TableVM> selection = (selected ? AllTables : AllDataItems.Values.Where(t => !t.Table.IsView)).ToList();
            foreach (var item in selection)
            {
                item.IsSelected = selected;
            }
        }

        private void SetViews(bool selected)
        {
            List<TableVM> selection = (selected ? AllViews : AllDataItems.Values.Where(t => t.Table.IsView)).ToList();
            foreach (var item in selection)
            {
                item.IsSelected = selected;
            }
        }

        private void SetDataItems()
        {
            foreach (var item in AllDataItems)
            {
                if (item.Value.IsSelected && item.Value.IsRelated)
                {
                    item.Value.IsSelected = false;
                }
            }
            foreach (var item in AllDataItems)
            {
                if (item.Value.IsSelected)
                {
                    SetDataItem(item.Value);
                }
            }
        }

        private void SetDataItem(TableVM tableVM)
        {
            if (!SetRelatedTables) return;

            var item = tableVM.Table;
            var selected = tableVM.IsSelected;
            var references = item.OuterKeys.Select(r => r.ColumnReferenced.Table)
                .Union(item.InnerKeys.Select(r => r.ColumnReferencing.Table))
                .Distinct();

            foreach (var table in references)
            {
                var tableToSee = AllDataItems[table.Name];
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

        private void AddToViews(TableVM element)
        {
            var viewToAdd = !element.IsVisible ? null : (element.Table.IsView) ? AllViews : AllTables;
            viewToAdd?.Add(element);
        }

        /// <summary>
        /// Handles the PropertyChanged event of the BaseVM control and adds a subscrition to changes on <see cref="MainWindowVM.SetRelatedTables"/>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void BaseVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.BaseVM_PropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case nameof(MainWindowVM.SetRelatedTables):
                    SetDataItems();
                    OnPropertyChanged(nameof(SetRelatedTables));
                    break;
                default:
                    break;
            }
        }

    }
}
