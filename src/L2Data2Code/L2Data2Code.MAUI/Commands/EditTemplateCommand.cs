using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Localize;
using L2Data2Code.Base;
using System;
using System.IO;
using L2Data2Code.MAUI.Commands.Interfaces;

namespace L2Data2Code.MAUI.Commands
{
    internal class EditTemplateCommand : DelegateCommand, IEditTemplateCommand
    {
        private readonly IGeneratorAdapter adapter;
        private readonly IMessageService messageService;
        private readonly IEditorLocatorService editorLocatorService;

        public EditTemplateCommand(IGeneratorAdapter adapter, IMessageService messageService, IEditorLocatorService editorLocatorService)
        {
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            this.editorLocatorService = editorLocatorService ?? throw new ArgumentNullException(nameof(editorLocatorService));
        }

        public override bool CanExecute(object parameter)
        {
            if (adapter.SelectedTemplate.IsEmpty())
            {
                return false;
            }

            try
            {
                var basePath = adapter.SettingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH].AddPathSeparator();
                var template = adapter.TemplatesConfiguration[adapter.SelectedTemplate].Path;
                var TemplatePath = Path.Combine(basePath, template);
                return Directory.Exists(TemplatePath);
            }
            catch (Exception ex)
            {
                messageService.Error($"GeneratorAdapter.CanEditTemplate() : {ex.Message}", Messages.ErrorGettingTemplatePath);
                return false;
            }
        }

        public override void Execute(object parameter)
        {
            var basePath = adapter.SettingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH].AddPathSeparator();
            var template = adapter.TemplatesConfiguration[adapter.SelectedTemplate].Path;
            var TemplatePath = Path.Combine(basePath, template);
            editorLocatorService.OpenWithEditor(TemplatePath);
        }
    }
}
