using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using System;
using System.IO;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Base;

namespace L2Data2Code.SharedContext.Commands
{
    public class OpenSettingsCommand : ReactiveBaseCommand, IOpenSettingsCommand
    {
        private readonly IEditorLocatorService editorLocatorService;

        public OpenSettingsCommand(IEditorLocatorService editorLocatorService, ICommandManager commandManager) : base(commandManager)
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
