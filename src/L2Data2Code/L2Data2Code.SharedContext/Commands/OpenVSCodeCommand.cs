using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2CodeUI.Shared.Adapters;
using System;
using System.IO;

namespace L2Data2Code.SharedContext.Commands
{
    public class OpenVSCodeCommand : ReactiveBaseCommand, IOpenVSCodeCommand
    {
        private readonly IEditorLocatorService editorLocatorService;

        public OpenVSCodeCommand(IEditorLocatorService editorLocatorService, ICommandManager commandManager) : base(commandManager)
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

        private static string GetPath(object parameter)
        {
            var slnFile = parameter as string;
            return Path.GetDirectoryName(slnFile);
        }
    }
}
