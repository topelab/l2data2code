using L2Data2Code.MAUI.Base;
using L2Data2Code.MAUI.Commands.Interfaces;
using L2Data2Code.MAUI.Main.TablePanel;
using L2Data2CodeUI.Shared.Adapters;
using System.ComponentModel;
using System.Windows.Input;

namespace L2Data2Code.MAUI.Commands
{
    internal class LoadTablesCommand : DelegateCommand, ILoadTablesCommand
    {
        private TablePanelVM controlVM;

        private readonly IGeneratorAdapter adapter;

        public LoadTablesCommand(IGeneratorAdapter adapter)
        {
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        public override void Execute(object parameter)
        {
            LoadAllTables();
        }

        public void Initialize(TablePanelVM controlVM)
        {
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

                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}