using L2Data2CodeUI.Shared.Adapters;
using System;
using System.IO;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Base;

namespace L2Data2Code.SharedContext.Commands
{
    public class OpenVSCommandFactory : DelegateCommandFactory<string>, IOpenVSCommandFactory
    {
        private readonly IAppService appService;

        public OpenVSCommandFactory(IAppService appService)
        {
            this.appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        protected override bool CanExecute(string slnFile)
        {
            return slnFile != null && slnFile.ToLower().EndsWith(".sln") && File.Exists(slnFile);
        }

        protected override void Execute(string slnFile)
        {
            appService.Open(slnFile);
        }
    }
}
