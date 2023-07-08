using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2CodeUI.Shared.Adapters;
using System;
using System.IO;

namespace L2Data2Code.SharedContext.Commands
{
    public class OpenVSCodeCommandFactory : DelegateCommandFactory<string>, IOpenVSCodeCommandFactory
    {
        private readonly IEditorLocatorService editorLocatorService;

        public OpenVSCodeCommandFactory(IEditorLocatorService editorLocatorService)
        {
            this.editorLocatorService = editorLocatorService ?? throw new ArgumentNullException(nameof(editorLocatorService));
        }

        protected override bool CanExecute(string slnFile)
        {
            return slnFile != null && Path.Exists(Path.GetDirectoryName(slnFile));
        }

        protected override void Execute(string slnFile)
        {
            var path = Path.GetDirectoryName(slnFile);
            editorLocatorService.OpenWithEditor(path);
        }
    }
}
