using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Main.TablePanel;
using L2Data2CodeUI.Shared.Adapters;
using NLog;
using System.Linq;

namespace L2Data2Code.SharedContext.Commands
{
    public class LoadTablesCommandFactory : DelegateCommandFactory, ILoadTablesCommandFactory
    {
        private TablePanelVM controlVM;

        private readonly IGeneratorAdapter adapter;
        private readonly ILogger logger;

        public LoadTablesCommandFactory(IGeneratorAdapter adapter, ILogger logger)
        {
            this.adapter = adapter ?? throw new System.ArgumentNullException(nameof(adapter));
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        protected override void Execute()
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
            logger.Info("Loading data base items");
            controlVM.AllDataItems.Clear();

            foreach (var item in adapter.Tables.OrderBy(k => k.Key))
            {
                TableVM element = new(item.Value);
                controlVM.AllDataItems.Add(element.Name, element);
            }
            logger.Info("All data base items loaded");
        }
    }
}
