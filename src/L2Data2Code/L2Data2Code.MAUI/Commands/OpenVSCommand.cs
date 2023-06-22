using L2Data2CodeUI.Shared.Adapters;
using L2Data2Code.MAUI.Base;
using System;
using System.IO;
using L2Data2Code.MAUI.Commands.Interfaces;

namespace L2Data2Code.MAUI.Commands
{
    internal class OpenVSCommand : DelegateCommand, IOpenVSCommand
    {
        private readonly IAppService appService;

        public OpenVSCommand(IAppService appService)
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
