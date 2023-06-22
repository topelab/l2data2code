using L2Data2Code.MAUI.Base;
using L2Data2Code.MAUI.Commands.Interfaces;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;

namespace L2Data2Code.MAUI.Commands
{
    internal class OpenSettingsCommand : DelegateCommand, IOpenSettingsCommand
    {
        private readonly IEditorLocatorService editorLocatorService;

        public OpenSettingsCommand(IEditorLocatorService editorLocatorService)
        {
            this.editorLocatorService = editorLocatorService ?? throw new ArgumentNullException(nameof(editorLocatorService));
        }

        public override bool CanExecute(object parameter)
        {
            var appBasePath = AppDomain.CurrentDomain.BaseDirectory.TrimPathSeparator();
            var config = $"{appBasePath}\\appsettings.json";
            return File.Exists(config);
        }

        public override void Execute(object parameter)
        {
            var appBasePath = AppDomain.CurrentDomain.BaseDirectory.TrimPathSeparator();
            var config = $"{appBasePath}\\appsettings.json";
            editorLocatorService.OpenWithEditor(config);
        }
    }
}
