using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using System;
using System.IO;

namespace L2Data2Code.SharedContext.Commands
{
    public class OpenSettingsCommandFactory : DelegateCommandFactory, IOpenSettingsCommandFactory
    {
        private readonly IEditorLocatorService editorLocatorService;

        public OpenSettingsCommandFactory(IEditorLocatorService editorLocatorService)
        {
            this.editorLocatorService = editorLocatorService ?? throw new ArgumentNullException(nameof(editorLocatorService));
        }

        protected override bool CanExecute()
        {
            var appBasePath = AppDomain.CurrentDomain.BaseDirectory.TrimPathSeparator();
            var config = $"{appBasePath}\\appsettings.json";
            return File.Exists(config);
        }

        protected override void Execute()
        {
            var appBasePath = AppDomain.CurrentDomain.BaseDirectory.TrimPathSeparator();
            var config = $"{appBasePath}\\appsettings.json";
            editorLocatorService.OpenWithEditor(config);
        }
    }
}
