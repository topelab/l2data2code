using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace L2Data2CodeWPF.Controls.TablePanel
{
    /// <summary>
    /// View model for table panel
    /// </summary>
    /// <seealso cref="L2Data2CodeWPF.Base.BaseControlViewModel" />
    public class TablePanelViewModel : BaseControlViewModel
    {
        private readonly IGeneratorAdapter adapter;
        private readonly MainWindowViewModel mainWindowViewModel;
        private Dispatcher dispatcher => Application.Current?.Dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="TablePanelViewModel"/> class.
        /// </summary>
        /// <param name="baseViewModel">The base view model.</param>
        public TablePanelViewModel(IBaseViewModel baseViewModel) : base(baseViewModel)
        {
            mainWindowViewModel = (MainWindowViewModel)baseViewModel;
            adapter = mainWindowViewModel.Adapter;
        }

        public ObservableCollection<TableViewModel> AllTables { get; set; } = new ObservableCollection<TableViewModel>();
        public ObservableCollection<TableViewModel> AllViews { get; set; } = new ObservableCollection<TableViewModel>();
        public Dictionary<string, TableViewModel> AllDataItems { get; set; } = new Dictionary<string, TableViewModel>();

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
        /// <remarks>Needs to be subcribed to changes on <see cref="MainWindowViewModel.SetRelatedTables"/></remarks>
        /// <value>
        ///   <c>true</c> if [set related tables]; otherwise, <c>false</c>.
        /// </value>
        public bool SetRelatedTables
        {
            get { return mainWindowViewModel.SetRelatedTables; }
            set { mainWindowViewModel.SetRelatedTables = value; }
        }

        /// <summary>
        /// Includes / exclude tables changed.
        /// </summary>
        public void IncludeExcludeTablesChanged()
        {
            Working = true;
            LoadingTables = true;
            ViewsVisible = false;
            var includeTables = adapter.ModulesConfiguration[mainWindowViewModel.SelectedModule].IncludeTables;
            var excludeTables = adapter.ModulesConfiguration[mainWindowViewModel.SelectedModule].ExcludeTables;
            var includeTablesRegex = includeTables == null ? null : new Regex(includeTables);
            var excludeTablesRegex = excludeTables == null ? null : new Regex(excludeTables);

            Task.Run(() =>
            {
                mainWindowViewModel.PauseTimer = true;
                PopulateDataItems(includeTablesRegex, excludeTablesRegex);
            }).ContinueWith((t) =>
            {
                dispatcher?.Invoke(() =>
                {
                    LoadingTables = false;
                    Working = false;
                    mainWindowViewModel.PauseTimer = false;
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
                var element = new TableViewModel(item.Value);
                element.PropertyChanged += TableViewModel_PropertyChanged;
                AllDataItems.Add(element.Name, element);
            }
            App.Logger.Info("All data base items loaded");
        }

        private void PopulateDataItems(Regex includeTablesRegex = null, Regex excludeTablesRegex = null)
        {
            App.Logger.Info("Populating tables and view lists");
            dispatcher.Invoke(() => ClearDataItemsLists());

            foreach (var element in AllDataItems.Values.OrderBy(k => k.Name))
            {
                element.IsVisible = includeTablesRegex == null || includeTablesRegex.IsMatch(element.Name);
                if (excludeTablesRegex != null && element.IsVisible)
                {
                    element.IsVisible = !excludeTablesRegex.IsMatch(element.Name);
                }
                dispatcher?.Invoke(() =>
                {
                    AddToViews(element);
                });
            }
            dispatcher?.Invoke(() => ViewsVisible = AllViews.Any());
            App.Logger.Info("All items populated");
        }

        private void TableViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TableViewModel.IsSelected))
            {
                var item = (TableViewModel)sender;
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

                mainWindowViewModel.OnPropertyChanged(nameof(mainWindowViewModel.GenerateCodeCommand));
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

        private void SetDataItem(TableViewModel tableVM)
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

        private void AddToViews(TableViewModel element)
        {
            var viewToAdd = !element.IsVisible ? null : (element.Table.IsView) ? AllViews : AllTables;
            viewToAdd?.Add(element);
        }

        /// <summary>
        /// Handles the PropertyChanged event of the BaseViewModel control and adds a subscrition to changes on <see cref="MainWindowViewModel.SetRelatedTables"/>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void BaseViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.BaseViewModel_PropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case nameof(MainWindowViewModel.SetRelatedTables):
                    SetDataItems();
                    OnPropertyChanged(nameof(SetRelatedTables));
                    break;
                default:
                    break;
            }
        }

    }
}
