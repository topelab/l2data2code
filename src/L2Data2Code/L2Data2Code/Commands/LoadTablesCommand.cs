using L2Data2Code.Commands.Interfaces;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2Code.Base;
using L2Data2Code.Main;
using System.ComponentModel;
using System.Linq;
using L2Data2Code.Main.TablePanel;

namespace L2Data2Code.Commands
{
    internal class LoadTablesCommand : DelegateCommand, ILoadTablesCommand
    {
        private MainWindowVM mainVM;
        private TablePanelVM controlVM;

        private readonly IGeneratorAdapter adapter;

        public LoadTablesCommand(IGeneratorAdapter adapter)
        {
            this.adapter = adapter ?? throw new System.ArgumentNullException(nameof(adapter));
        }

        public override void Execute(object parameter)
        {
            LoadAllTables();
        }

        public void Initialize(MainWindowVM mainVM, TablePanelVM controlVM)
        {
            this.mainVM = mainVM;
            this.controlVM = controlVM;
        }

        /// <summary>
        /// Loads all tables.
        /// </summary>
        private void LoadAllTables()
        {
            App.Logger.Info("Loading data base items");
            controlVM.AllDataItems.Values.ToList().ForEach(e => e.PropertyChanged -= OnTableVMPropertyChanged);
            controlVM.AllDataItems.Clear();

            foreach (var item in adapter.Tables.OrderBy(k => k.Key))
            {
                TableVM element = new(item.Value);
                element.PropertyChanged += OnTableVMPropertyChanged;
                controlVM.AllDataItems.Add(element.Name, element);
            }
            App.Logger.Info("All data base items loaded");
        }


        private void OnTableVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TableVM.IsSelected))
            {
                var item = (TableVM)sender;
                if (item.IsSelected)
                {
                    App.Logger.Trace($"*** Table: {item.Name} has been selected");
                    controlVM.SetDataItemCommand.Execute(item);
                }
                else
                {
                    App.Logger.Trace($"*** Table: {item.Name} has unselected");
                    item.IsRelated = false;
                }

                mainVM.OnPropertyChanged(nameof(mainVM.GenerateCodeCommand));
            }
        }
    }
}
