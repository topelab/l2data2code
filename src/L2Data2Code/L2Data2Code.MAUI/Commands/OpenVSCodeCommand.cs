using L2Data2Code.MAUI.Base;
using L2Data2Code.MAUI.Commands.Interfaces;
using L2Data2CodeUI.Shared.Adapters;

namespace L2Data2Code.MAUI.Commands
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

        private static string GetPath(object parameter)
        {
            var slnFile = parameter as string;
            return Path.GetDirectoryName(slnFile);
        }
    }
}
