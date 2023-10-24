using L2Data2Code.SharedLib.Extensions;
using System.IO;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Base;

namespace L2Data2Code.SharedContext.Commands
{
    public class OpenFolderCommandFactory : DelegateCommandFactory<string>, IOpenFolderCommandFactory
    {
        private readonly IProcessManager processManager;

        public OpenFolderCommandFactory(IProcessManager processManager)
        {
            this.processManager = processManager ?? throw new System.ArgumentNullException(nameof(processManager));
        }

        protected override bool CanExecute(string slnFile)
        {
            return Directory.Exists(Path.GetDirectoryName(slnFile));
        }

        protected override void Execute(string slnFile)
        {
            processManager.Run(Path.GetDirectoryName(slnFile));
        }
    }
}
