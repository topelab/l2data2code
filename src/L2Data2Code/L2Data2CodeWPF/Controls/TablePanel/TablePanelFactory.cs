using L2Data2CodeWPF.Base;
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

        public TablePanelFactory(ITablePanelBindManager bindManager, ILoadTablesCommand loadTablesCommand, ISetDataItemsCommand setDataItemsCommand)
        {
            this.bindManager = bindManager ?? throw new ArgumentNullException(nameof(bindManager));
            this.loadTablesCommand = loadTablesCommand ?? throw new ArgumentNullException(nameof(loadTablesCommand));
            this.setDataItemsCommand = setDataItemsCommand ?? throw new ArgumentNullException(nameof(setDataItemsCommand));
        }

        public TablePanelVM Create(MainWindowVM mainVM)
        {
            TablePanelVM tablePanelVM = new();
            loadTablesCommand.Initialize(mainVM, tablePanelVM);
            setDataItemsCommand.Initialize(tablePanelVM);
            tablePanelVM.SetCommands(loadTablesCommand, setDataItemsCommand);
            bindManager.Start(mainVM, tablePanelVM);

            return tablePanelVM;
        }
    }
}
