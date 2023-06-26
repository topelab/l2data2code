using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Main.TablePanel;
using L2Data2CodeUI.Shared.Adapters;
using NLog;
using System.ComponentModel;
using System.Linq;

namespace L2Data2Code.SharedContext.Commands
{
    public class LoadTablesCommand : ReactiveBaseCommand, ILoadTablesCommand
    {
        private TablePanelVM controlVM;

        private readonly IGeneratorAdapter adapter;
        private readonly ILogger logger;

        public LoadTablesCommand(IGeneratorAdapter adapter, ILogger logger, ICommandManager commandManager) : base(commandManager)
        {
            this.adapter = adapter ?? throw new System.ArgumentNullException(nameof(adapter));
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public override void Execute(object parameter)
        {
            LoadAllTables();
        }

        public override bool CanExecute(object parameter)
        {
            return true;
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
            logger.Info("Loading data base items");
            controlVM.AllDataItems.Values.ToList().ForEach(e => e.PropertyChanged -= OnTableVMPropertyChanged);
            controlVM.AllDataItems.Clear();

            foreach (var item in adapter.Tables.OrderBy(k => k.Key))
            {
                TableVM element = new(item.Value);
                element.PropertyChanged += OnTableVMPropertyChanged;
                controlVM.AllDataItems.Add(element.Name, element);
            }
            logger.Info("All data base items loaded");
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

                commandManager.InvalidateRequerySuggested();
            }
        }
    }
}
