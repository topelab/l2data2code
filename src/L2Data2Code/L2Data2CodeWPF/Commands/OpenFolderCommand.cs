using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.Commands.Interfaces;
using System.IO;

namespace L2Data2CodeWPF.Commands
{
    internal class OpenFolderCommand : DelegateCommand, IOpenFolderCommand
    {
        private IBaseVM viewModel;
        private readonly IProcessManager processManager;

        public OpenFolderCommand(IProcessManager processManager)
        {
            this.processManager = processManager ?? throw new System.ArgumentNullException(nameof(processManager));
        }

        public void SetViewModel(IBaseVM viewModel)
        {
            this.viewModel = viewModel ?? throw new System.ArgumentNullException(nameof(viewModel));
        }

        public override bool CanExecute(object parameter)
        {
            var slnFile = parameter as string;
            return Directory.Exists(Path.GetDirectoryName(slnFile));
        }

        public override void Execute(object parameter)
        {
            var slnFile = parameter as string;
            viewModel.Working = true;
            processManager.Run(Path.GetDirectoryName(slnFile));
            viewModel.Working = false;
        }
    }
}
