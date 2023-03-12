using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.Commands.Interfaces;
using System;
using System.IO;

namespace L2Data2CodeWPF.Commands
{
    internal class OpenVSCodeCommand : DelegateCommand, IOpenVSCodeCommand
    {
        private readonly IEditorLocatorService editorLocatorService;

        public OpenVSCodeCommand(IEditorLocatorService editorLocatorService)
        {
            this.editorLocatorService = editorLocatorService ?? throw new ArgumentNullException(nameof(editorLocatorService));
        }

        public override bool CanExecute(object parameter)
        {
            var path = GetPath(parameter);
            return Path.Exists(path);
        }

        public override void Execute(object parameter)
        {
            var path = GetPath(parameter);
            editorLocatorService.OpenWithEditor(path);
        }

        private string GetPath(object parameter)
        {
            var slnFile = parameter as string;
            return Path.GetDirectoryName(slnFile);
        }
    }
}
