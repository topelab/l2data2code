using L2Data2Code.SharedLib.Configuration;
using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Entities
{
    public class CodeGeneratorDto
    {
        public const string DefaultOutputPath = "c:\\src\\tmp\\";

        public string TemplatePath { get; set; }
        public string TemplateResource { get; set; }
        public string SchemaName { get; set; }
        public string DescriptionsSchemaName { get; set; }
        public string OutputPath { get; set; }
        public List<string> TableList { get; set; }
        public string Company { get; set; }
        public string Area { get; set; }
        public string Module { get; set; }
        public string UserVariables { get; set; }
        public bool NoEntities { get; set; }
        public bool GenerateReferenced { get; set; }
        public bool RemoveFolders { get; set; }
        public string CreatedFromSchemaName { get; set; }
        public bool CleanEndOfCodeLine { get; set; } = true;
        public bool LastPass { get; set; }
        public string GeneratorApplication { get; set; }
        public string GeneratorVersion { get; set; }
        public bool GenerateJsonInfo { get; set; }
        public string JsonGeneratedPath { get; set; }
        public bool GeneateOnlyJson { get; set; }
        public string Encoding { get; set; }
        public string EndOfLine { get; set; }
        public IBasicConfiguration<SchemaConfiguration> SchemasConfiguration { get; set; }
    }
}
