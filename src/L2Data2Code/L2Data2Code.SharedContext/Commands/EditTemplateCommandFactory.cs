using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Localize;
using System;
using System.IO;

namespace L2Data2Code.SharedContext.Commands
{
    public class EditTemplateCommandFactory : DelegateCommandFactory, IEditTemplateCommandFactory
    {
        private readonly IGeneratorAdapter adapter;
        private readonly IMessageService messageService;
        private readonly IEditorLocatorService editorLocatorService;

        public EditTemplateCommandFactory(IGeneratorAdapter adapter, IMessageService messageService, IEditorLocatorService editorLocatorService)
        {
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            this.editorLocatorService = editorLocatorService ?? throw new ArgumentNullException(nameof(editorLocatorService));
        }

        protected override bool CanExecute()
        {
            if (adapter.SelectedTemplate == null)
            {
                return false;
            }

            try
            {
                var basePath = adapter.SettingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH].AddPathSeparator();
                var template = adapter.SelectedTemplate.Path;
                var TemplatePath = Path.Combine(basePath, template);
                return Directory.Exists(TemplatePath);
            }
            catch (Exception ex)
            {
                messageService.Error($"GeneratorAdapter.CanEditTemplate() : {ex.Message}", Messages.ErrorGettingTemplatePath);
                return false;
            }
        }

        protected override void Execute()
        {
            var basePath = adapter.SettingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH].AddPathSeparator();
            var template = adapter.SelectedTemplate.Path;
            var TemplatePath = Path.Combine(basePath, template);
            editorLocatorService.OpenWithEditor(TemplatePath);
        }
    }
}
