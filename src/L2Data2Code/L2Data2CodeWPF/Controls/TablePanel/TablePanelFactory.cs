using L2Data2CodeWPF.Commands.Interfaces;
using L2Data2CodeWPF.Main;
using System;

namespace L2Data2CodeWPF.Controls.TablePanel
{
    internal class TablePanelFactory : ITablePanelFactory
    {
        private readonly ITablePanelBindManager bindManager;
        private readonly ILoadTablesCommand loadTablesCommand;
        private readonly ISetDataItemsCommand setDataItemsCommand;
        private readonly ISetDataItemCommand setDataItemCommand;

        public TablePanelFactory(ITablePanelBindManager bindManager, ILoadTablesCommand loadTablesCommand, ISetDataItemsCommand setDataItemsCommand, ISetDataItemCommand setDataItemCommand)
        {
            this.bindManager = bindManager ?? throw new ArgumentNullException(nameof(bindManager));
            this.loadTablesCommand = loadTablesCommand ?? throw new ArgumentNullException(nameof(loadTablesCommand));
            this.setDataItemsCommand = setDataItemsCommand ?? throw new ArgumentNullException(nameof(setDataItemsCommand));
            this.setDataItemCommand = setDataItemCommand ?? throw new ArgumentNullException(nameof(setDataItemCommand));
        }

        public TablePanelVM Create(MainWindowVM mainVM)
        {
            TablePanelVM tablePanelVM = new();
            loadTablesCommand.Initialize(mainVM, tablePanelVM);
            setDataItemsCommand.Initialize(tablePanelVM);
            setDataItemCommand.Initialize(tablePanelVM);
            tablePanelVM.SetCommands(loadTablesCommand, setDataItemsCommand, setDataItemCommand);
            bindManager.Start(mainVM, tablePanelVM);

            return tablePanelVM;
        }
    }
}
