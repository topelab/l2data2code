using L2Data2Code.SharedLib.Extensions;
using System.IO;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Base;

namespace L2Data2Code.SharedContext.Commands
{
    internal class OpenFolderCommand : ReactiveBaseCommand, IOpenFolderCommand
    {
        private readonly IProcessManager processManager;

        public OpenFolderCommand(IProcessManager processManager, ICommandManager commandManager) : base(commandManager)
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
