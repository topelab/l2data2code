using L2Data2Code.SharedContext.Commands.Interfaces;
using System;

namespace L2Data2Code.SharedContext.Main.TablePanel
{
    internal class TablePanelFactory : ITablePanelFactory
    {
        private readonly ITablePanelChangeListener bindManager;
        private readonly ILoadTablesCommand loadTablesCommand;
        private readonly ISetDataItemsCommand setDataItemsCommand;
        private readonly ISetDataItemCommand setDataItemCommand;

        public TablePanelFactory(ITablePanelChangeListener bindManager, ILoadTablesCommand loadTablesCommand, ISetDataItemsCommand setDataItemsCommand, ISetDataItemCommand setDataItemCommand)
        {
            this.bindManager = bindManager ?? throw new ArgumentNullException(nameof(bindManager));
            this.loadTablesCommand = loadTablesCommand ?? throw new ArgumentNullException(nameof(loadTablesCommand));
            this.setDataItemsCommand = setDataItemsCommand ?? throw new ArgumentNullException(nameof(setDataItemsCommand));
            this.setDataItemCommand = setDataItemCommand ?? throw new ArgumentNullException(nameof(setDataItemCommand));
        }

        public TablePanelVM Create(MainWindowVM mainVM)
        {
            TablePanelVM tablePanelVM = new();
            loadTablesCommand.Initialize(tablePanelVM);
            setDataItemsCommand.Initialize(tablePanelVM);
            setDataItemCommand.Initialize(tablePanelVM);
            tablePanelVM.SetCommands(loadTablesCommand, setDataItemsCommand, setDataItemCommand);
            bindManager.Start(mainVM, tablePanelVM);

            return tablePanelVM;
        }
    }
}
