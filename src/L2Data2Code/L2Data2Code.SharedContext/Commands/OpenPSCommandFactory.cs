using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using System.IO;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Base;

namespace L2Data2Code.SharedContext.Commands
{
    public class OpenPSCommandFactory : DelegateCommandFactory<string>, IOpenPSCommandFactory
    {
        private readonly IAppService appService;
        private readonly IProcessManager processManager;

        public OpenPSCommandFactory(IAppService appService, IProcessManager processManager)
        {
            this.appService = appService ?? throw new System.ArgumentNullException(nameof(appService));
            this.processManager = processManager ?? throw new System.ArgumentNullException(nameof(processManager));
        }

        protected override bool CanExecute(string slnFile)
        {
            return slnFile != null && Path.Exists(Path.GetDirectoryName(slnFile));
        }

        protected override void Execute(string slnFile)
        {
            appService.Open(slnFile, processManager.FindPS(), $"-noexit -command \"cd {AppService.DIRECTORY_PATTERN}\"");
        }
    }
}
