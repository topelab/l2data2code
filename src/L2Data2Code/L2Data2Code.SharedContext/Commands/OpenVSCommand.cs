using L2Data2CodeUI.Shared.Adapters;
using System;
using System.IO;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Base;

namespace L2Data2Code.SharedContext.Commands
{
    public class OpenVSCommand : ReactiveBaseCommand, IOpenVSCommand
    {
        private readonly IAppService appService;

        public OpenVSCommand(IAppService appService, ICommandManager commandManager) : base(commandManager)
        {
            this.appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        public override bool CanExecute(object parameter)
        {
            return parameter is string slnFile && slnFile.ToLower().EndsWith(".sln") && File.Exists(slnFile);
        }

        public override void Execute(object parameter)
        {
            var slnFile = parameter as string;
            appService.Open(slnFile);
        }
    }
}
