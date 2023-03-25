using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.Commands.Interfaces;
using L2Data2CodeWPF.Main;
using System;

namespace L2Data2CodeWPF.Controls.CommandBar
{
    internal class CommandBarFactory : ICommandBarFactory
    {
        private readonly ICommandBarBindManager bindManager;
        private readonly IOpenFolderCommand openFolderCommand;
        private readonly IEditTemplateCommand editTemplateCommand;
        private readonly IOpenSettingsCommand openSettingsCommand;
        private readonly IOpenVSCommand openVSCommand;
        private readonly IOpenVSCodeCommand openVSCodeCommand;
        private readonly IOpenPSCommand openPSCommand;
        private readonly IOpenVarsWindowCommand openVarsWindowCommand;

        public CommandBarFactory(ICommandBarBindManager bindManager,
                                 IOpenFolderCommand openFolderCommand,
                                 IEditTemplateCommand editTemplateCommand,
                                 IOpenSettingsCommand openSettingsCommand,
                                 IOpenVSCommand openVSCommand,
                                 IOpenVSCodeCommand openVSCodeCommand,
                                 IOpenPSCommand openPSCommand,
                                 IOpenVarsWindowCommand openVarsWindowCommand)
        {
            this.bindManager = bindManager ?? throw new ArgumentNullException(nameof(bindManager));
            this.openFolderCommand = openFolderCommand ?? throw new ArgumentNullException(nameof(openFolderCommand));
            this.editTemplateCommand = editTemplateCommand ?? throw new ArgumentNullException(nameof(editTemplateCommand));
            this.openSettingsCommand = openSettingsCommand ?? throw new ArgumentNullException(nameof(openSettingsCommand));
            this.openVSCommand = openVSCommand ?? throw new ArgumentNullException(nameof(openVSCommand));
            this.openVSCodeCommand = openVSCodeCommand ?? throw new ArgumentNullException(nameof(openVSCodeCommand));
            this.openPSCommand = openPSCommand ?? throw new ArgumentNullException(nameof(openPSCommand));
            this.openVarsWindowCommand = openVarsWindowCommand ?? throw new ArgumentNullException(nameof(openVarsWindowCommand));
        }

        public CommandBarVM Create(MainWindowVM mainVM)
        {
            CommandBarVM commandBarVM = new();
            commandBarVM.SetCommands(openFolderCommand, editTemplateCommand, openSettingsCommand, openVSCommand, openVSCodeCommand, openPSCommand, openVarsWindowCommand);
            bindManager.Start(mainVM, commandBarVM);

            return commandBarVM;
        }
    }
}
