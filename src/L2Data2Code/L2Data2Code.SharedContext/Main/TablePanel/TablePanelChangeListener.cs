using L2Data2Code.BaseGenerator.Configuration;
using L2Data2Code.SharedContext.Base;
using L2Data2CodeUI.Shared.Adapters;
using NLog;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace L2Data2Code.SharedContext.Main.TablePanel
{
    public class TablePanelChangeListener : ITablePanelChangeListener
    {
        private readonly IGeneratorAdapter adapter;
        private readonly ILogger logger;

        public TablePanelChangeListener(IGeneratorAdapter adapter, ILogger logger)
        {
            this.adapter = adapter ?? throw new System.ArgumentNullException(nameof(adapter));
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        private MainWindowVM mainVM;
        private TablePanelVM controlVM;

        public void Start(MainWindowVM mainVM, TablePanelVM controlVM)
        {
            Stop();
            this.mainVM = mainVM;
            this.controlVM = controlVM;

            this.mainVM.PropertyChanged += OnParentVMPropertyChanged;
            this.controlVM.AllTables.CollectionChanged += OnAllTablesCollectionChanged;
        }

        private void OnAllTablesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.OfType<TableVM>().ToList().ForEach(e => e.PropertyChanged += OnTableVMPropertyChanged);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.OfType<TableVM>().ToList().ForEach(e => e.PropertyChanged -= OnTableVMPropertyChanged);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    e.OldItems.OfType<TableVM>().ToList().ForEach(e => e.PropertyChanged -= OnTableVMPropertyChanged);
                    e.NewItems.OfType<TableVM>().ToList().ForEach(e => e.PropertyChanged += OnTableVMPropertyChanged);
                    break;
            }
        }

        private void OnTableVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TableVM.IsSelected))
            {
                var item = (TableVM)sender;
                if (item.IsSelected)
                {
                    logger.Trace($"*** Table: {item.Name} has been selected");
                    controlVM.SetDataItemCommand.Execute(item);
                }
                else
                {
                    logger.Trace($"*** Table: {item.Name} has unselected");
                    item.IsRelated = false;
                }
                mainVM.GenerateCodeCommand.RaiseCanExecuteChanged();
            }
        }


        public void Stop()
        {
            if (mainVM != null)
            {
                mainVM.PropertyChanged -= OnParentVMPropertyChanged;
            }
        }

        private void OnParentVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MainWindowVM.Working):
                    controlVM.Working = mainVM.Working;
                    break;
                case nameof(MainWindowVM.SetRelatedTables):
                    controlVM.SetRelatedTables = mainVM.SetRelatedTables;
                    controlVM.SetDataItemsCommand.Execute(null);
                    mainVM.GenerateCodeCommand.RaiseCanExecuteChanged();
                    break;
                case nameof(MainWindowVM.SelectedModule):
                    if (mainVM.SelectedModule != null)
                    {
                        IncludeExcludeTablesChanged(mainVM.SelectedModule);
                        controlVM.OnPropertyChanged(nameof(controlVM.AllTables));
                        controlVM.OnPropertyChanged(nameof(controlVM.AllViews));
                    }
                    break;
                default:
                    break;
            }
        }

        private void IncludeExcludeTablesChanged(ModuleConfiguration selectedModule)
        {
            controlVM.WorkOnAction(() =>
            {
                controlVM.LoadingTables = true;
                controlVM.ViewsVisible = false;
                var includeTables = selectedModule.IncludeTables;
                var excludeTables = selectedModule.ExcludeTables;
                var includeTablesRegex = includeTables == null ? null : new Regex(includeTables);
                var excludeTablesRegex = excludeTables == null ? null : new Regex(excludeTables);

                mainVM.PauseTimer = true;
                PopulateDataItems(includeTablesRegex, excludeTablesRegex);
                controlVM.LoadingTables = false;
                mainVM.PauseTimer = false;
            });
        }

        private void PopulateDataItems(Regex includeTablesRegex = null, Regex excludeTablesRegex = null)
        {
            logger.Info("Populating tables and view lists");
            ClearDataItemsLists();

            foreach (var element in controlVM.AllDataItems.Values.OrderBy(k => k.Name))
            {
                element.IsVisible = includeTablesRegex == null || includeTablesRegex.IsMatch(element.Name);
                if (excludeTablesRegex != null && element.IsVisible)
                {
                    element.IsVisible = !excludeTablesRegex.IsMatch(element.Name);
                }
                controlVM.AddToViews(element);
            }
            controlVM.ViewsVisible = controlVM.AllViews.Any();
            if (controlVM.ViewsVisible)
            {
                controlVM.AutoHeight = "50*";
            }
            else
            {
                controlVM.AutoHeight = "*";
            }
            logger.Info("All items populated");
        }

        void ClearDataItemsLists()
        {
            controlVM.AllTables.Clear();
            controlVM.AllViews.Clear();

            controlVM.SelectAllTables = false;
            controlVM.SelectAllViews = false;
        }
    }
}
