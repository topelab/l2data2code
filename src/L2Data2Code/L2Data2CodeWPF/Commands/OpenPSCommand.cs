using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.Commands.Interfaces;
using System.IO;

namespace L2Data2CodeWPF.Commands
{
    internal class OpenPSCommand : DelegateCommand, IOpenPSCommand
    {
        private readonly IAppService appService;
        private readonly IProcessManager processManager;

        public OpenPSCommand(IAppService appService, IProcessManager processManager)
        {
            this.appService = appService ?? throw new System.ArgumentNullException(nameof(appService));
            this.processManager = processManager ?? throw new System.ArgumentNullException(nameof(processManager));
        }

        public override bool CanExecute(object parameter)
        {
            var slnFile = parameter as string;
            return File.Exists(slnFile);
        }

        public override void Execute(object parameter)
        {
            var slnFile = parameter as string;
            var path = Path.GetDirectoryName(slnFile);
            appService.Open(slnFile, processManager.FindPS(), $"-noexit -command \"cd {AppService.DIRECTORY_PATTERN}\"");
        }
    }
}
