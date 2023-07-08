using L2Data2Code.SharedContext.Commands.Interfaces;
using System;

namespace L2Data2Code.SharedContext.Main.CommandBar
{
    public class CommandBarFactory : ICommandBarFactory
    {
        private readonly ICommandBarChangeListener bindManager;
        private readonly IOpenFolderCommandFactory openFolderCommandFactory;
        private readonly IEditTemplateCommandFactory editTemplateCommandFactory;
        private readonly IOpenSettingsCommandFactory openSettingsCommandFactory;
        private readonly IOpenVSCommandFactory openVSCommandFactory;
        private readonly IOpenVSCodeCommandFactory openVSCodeCommandFactory;
        private readonly IOpenPSCommandFactory openPSCommandFactory;
        private readonly IOpenVarsWindowCommandFactory openVarsWindowCommandFactory;

        public CommandBarFactory(ICommandBarChangeListener bindManager,
                                 IOpenFolderCommandFactory openFolderCommandFactory,
                                 IEditTemplateCommandFactory editTemplateCommandFactory,
                                 IOpenSettingsCommandFactory openSettingsCommandFactory,
                                 IOpenVSCommandFactory openVSCommandFactory,
                                 IOpenVSCodeCommandFactory openVSCodeCommandFactory,
                                 IOpenPSCommandFactory openPSCommandFactory,
                                 IOpenVarsWindowCommandFactory openVarsWindowCommandFactory)
        {
            this.bindManager = bindManager ?? throw new ArgumentNullException(nameof(bindManager));
            this.openFolderCommandFactory = openFolderCommandFactory ?? throw new ArgumentNullException(nameof(openFolderCommandFactory));
            this.editTemplateCommandFactory = editTemplateCommandFactory ?? throw new ArgumentNullException(nameof(editTemplateCommandFactory));
            this.openSettingsCommandFactory = openSettingsCommandFactory ?? throw new ArgumentNullException(nameof(openSettingsCommandFactory));
            this.openVSCommandFactory = openVSCommandFactory ?? throw new ArgumentNullException(nameof(openVSCommandFactory));
            this.openVSCodeCommandFactory = openVSCodeCommandFactory ?? throw new ArgumentNullException(nameof(openVSCodeCommandFactory));
            this.openPSCommandFactory = openPSCommandFactory ?? throw new ArgumentNullException(nameof(openPSCommandFactory));
            this.openVarsWindowCommandFactory = openVarsWindowCommandFactory ?? throw new ArgumentNullException(nameof(openVarsWindowCommandFactory));
        }

        public CommandBarVM Create(MainWindowVM mainVM)
        {
            CommandBarVM commandBarVM = new();
            commandBarVM.SetCommands(openFolderCommandFactory.Create(),
                                     editTemplateCommandFactory.Create(),
                                     openSettingsCommandFactory.Create(),
                                     openVSCommandFactory.Create(),
                                     openVSCodeCommandFactory.Create(),
                                     openPSCommandFactory.Create(),
                                     openVarsWindowCommandFactory.Create());
            bindManager.Start(mainVM, commandBarVM);

            return commandBarVM;
        }
    }
}
