using L2Data2CodeUI.Shared.Adapters;
using L2Data2Code.Main;
using L2Data2Code.SharedLib;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace L2Data2Code.Main.TablePanel
{
    internal class TablePanelBindManager : ITablePanelBindManager
    {
        private readonly IGeneratorAdapter adapter;
        private readonly IDispatcherWrapper dispatcher;

        public TablePanelBindManager(IGeneratorAdapter adapter, IDispatcherWrapper dispatcher)
        {
            this.adapter = adapter ?? throw new System.ArgumentNullException(nameof(adapter));
            this.dispatcher = dispatcher ?? throw new System.ArgumentNullException(nameof(dispatcher));
        }

        private MainWindowVM mainVM;
        private TablePanelVM controlVM;

        public void Start(MainWindowVM mainVM, TablePanelVM controlVM)
        {
            Stop();
            this.mainVM = mainVM;
            this.controlVM = controlVM;

            this.mainVM.PropertyChanged += OnParentVMPropertyChanged;
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

        private void IncludeExcludeTablesChanged(string selectedModule)
        {
            controlVM.Working = true;
            controlVM.LoadingTables = true;
            controlVM.ViewsVisible = false;
            var includeTables = adapter.ModulesConfiguration[selectedModule].IncludeTables;
            var excludeTables = adapter.ModulesConfiguration[selectedModule].ExcludeTables;
            var includeTablesRegex = includeTables == null ? null : new Regex(includeTables);
            var excludeTablesRegex = excludeTables == null ? null : new Regex(excludeTables);

            Task.Run(() =>
            {
                mainVM.PauseTimer = true;
                PopulateDataItems(includeTablesRegex, excludeTablesRegex);
            }).ContinueWith((t) =>
            {
                dispatcher.Invoke(() =>
                {
                    controlVM.LoadingTables = false;
                    controlVM.Working = false;
                    mainVM.PauseTimer = false;
                });
            });
        }

        private void PopulateDataItems(Regex includeTablesRegex = null, Regex excludeTablesRegex = null)
        {
            App.Logger.Info("Populating tables and view lists");
            dispatcher.Invoke(ClearDataItemsLists);

            foreach (var element in controlVM.AllDataItems.Values.OrderBy(k => k.Name))
            {
                element.IsVisible = includeTablesRegex == null || includeTablesRegex.IsMatch(element.Name);
                if (excludeTablesRegex != null && element.IsVisible)
                {
                    element.IsVisible = !excludeTablesRegex.IsMatch(element.Name);
                }
                dispatcher.Invoke(controlVM.AddToViews, element);
            }
            dispatcher.Invoke(() => controlVM.ViewsVisible = controlVM.AllViews.Any());
            App.Logger.Info("All items populated");
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