using L2Data2Code.BaseGenerator.Configuration;
using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Configuration;
using L2Data2CodeUI.Shared.Dto;
using System;
using System.Collections.Generic;

namespace L2Data2CodeUI.Shared.Adapters
{
    public interface IGeneratorAdapter
    {
        Action OnConfigurationChanged { get; set; }
        IBasicNameValueConfiguration SettingsConfiguration { get; }
        IDataSorcesConfiguration DataSourcesConfiguration { get; }
        IBasicConfiguration<ModuleConfiguration> ModulesConfiguration { get; }
        IBasicConfiguration<SchemaConfiguration> SchemasConfiguration { get; }
        ITemplatesConfiguration TemplatesConfiguration { get; }
        Dictionary<string, object> CompiledVars { get; }
        string GeneratorApplication { get; set; }
        string GeneratorVersion { get; set; }
        string InputSourceType { get; }
        string OutputPath { get; set; }
        DataSourceConfiguration SelectedDataSource { get; }
        ModuleConfiguration SelectedModule { get; }
        TemplateConfiguration SelectedTemplate { get; }
        Setting SelectedSetting { get; }
        string SlnFile { get; }
        string SolutionType { get; set; }
        Tables Tables { get; }
        AppType AppType { get; }

        IEnumerable<Table> GetAllTables();
        IEnumerable<DataSourceConfiguration> GetAreaList();
        IEnumerable<ModuleConfiguration> GetModuleList(DataSourceConfiguration selectedDataSource);
        IEnumerable<TemplateConfiguration> GetTemplateList();
        IEnumerable<Setting> GetSettings(TemplateConfiguration selectedTemplate, DataSourceConfiguration selectedDataSource = null);
        void SetCurrentDataSource(DataSourceConfiguration selectedDataSource);
        void SetCurrentModule(ModuleConfiguration selectedModule, bool triggered = false);
        void SetCurrentTemplate(TemplateConfiguration selectedTemplate, bool triggered = false);
        void SetCurrentSetting(Setting setting, bool triggered = false);
        void Run(CodeGeneratorDto baseOptions);
        ModuleConfiguration GetDefaultModule(DataSourceConfiguration selectedDataSource);
    }
}