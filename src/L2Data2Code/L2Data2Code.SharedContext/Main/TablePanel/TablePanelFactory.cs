using L2Data2Code.SharedContext.Commands.Interfaces;
using System;

namespace L2Data2Code.SharedContext.Main.TablePanel
{
    public class TablePanelFactory : ITablePanelFactory
    {
        private readonly ITablePanelChangeListener bindManager;
        private readonly ILoadTablesCommandFactory loadTablesCommandFactory;
        private readonly ISetDataItemsCommandFactory setDataItemsCommandFactory;
        private readonly ISetDataItemCommandFactory setDataItemCommandFactory;

        public TablePanelFactory(ITablePanelChangeListener bindManager, ILoadTablesCommandFactory loadTablesCommandFactory, ISetDataItemsCommandFactory setDataItemsCommandFactory, ISetDataItemCommandFactory setDataItemCommandFactory)
        {
            this.bindManager = bindManager ?? throw new ArgumentNullException(nameof(bindManager));
            this.loadTablesCommandFactory = loadTablesCommandFactory ?? throw new ArgumentNullException(nameof(loadTablesCommandFactory));
            this.setDataItemsCommandFactory = setDataItemsCommandFactory ?? throw new ArgumentNullException(nameof(setDataItemsCommandFactory));
            this.setDataItemCommandFactory = setDataItemCommandFactory ?? throw new ArgumentNullException(nameof(setDataItemCommandFactory));
        }

        public TablePanelVM Create(MainWindowVM mainVM)
        {
            TablePanelVM tablePanelVM = new();
            loadTablesCommandFactory.Initialize(tablePanelVM);
            setDataItemsCommandFactory.Initialize(tablePanelVM);
            setDataItemCommandFactory.Initialize(tablePanelVM);
            tablePanelVM.SetCommands(loadTablesCommandFactory.Create(), setDataItemsCommandFactory.Create(), setDataItemCommandFactory.Create());
            bindManager.Start(mainVM, tablePanelVM);

            return tablePanelVM;
        }
    }
}
