using L2Data2Code.Commands.Interfaces;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.Base;
using System.IO;

namespace L2Data2Code.Commands
{
    internal class OpenFolderCommand : DelegateCommand, IOpenFolderCommand
    {
        private readonly IProcessManager processManager;

        public OpenFolderCommand(IProcessManager processManager)
        {
            this.processManager = processManager ?? throw new System.ArgumentNullException(nameof(processManager));
        }

        public override bool CanExecute(object parameter)
        {
            var slnFile = parameter as string;
            return Directory.Exists(Path.GetDirectoryName(slnFile));
        }

        public override void Execute(object parameter)
        {
            var slnFile = parameter as string;
            processManager.Run(Path.GetDirectoryName(slnFile));
        }
    }
}
