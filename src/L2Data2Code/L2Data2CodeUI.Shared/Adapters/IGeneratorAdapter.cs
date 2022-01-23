using L2Data2Code.BaseGenerator.Configuration;
using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Configuration;
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
        string SelectedDataSource { get; }
        string SelectedModule { get; }
        string SelectedTemplate { get; }
        string SelectedVars { get; }
        string SlnFile { get; }
        string SolutionType { get; set; }
        Tables Tables { get; }
        IEnumerable<Table> GetAllTables();
        IEnumerable<string> GetAreaList();
        IEnumerable<string> GetModuleList(string selectedDataSource);
        IEnumerable<string> GetTemplateList();
        IEnumerable<string> GetVarsList(string selectedTemplate);
        void SetCurrentDataSource(string selectedDataSource);
        void SetCurrentModule(string selectedModule, bool triggered = false);
        void SetCurrentTemplate(string selectedTemplate, bool triggered = false);
        void SetCurrentVars(string selectedVars, bool triggered = false);
        void Run(CodeGeneratorDto baseOptions);
    }
}