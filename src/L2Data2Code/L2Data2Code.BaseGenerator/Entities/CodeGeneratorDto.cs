using L2Data2Code.BaseGenerator.Configuration;
using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SharedLib.Configuration;
using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Entities
{
    /// <summary>
    /// DTO for code generator options
    /// </summary>
    public class CodeGeneratorDto : ISchemaOptions
    {
        /// <summary>
        /// Default output path
        /// </summary>
        public const string DefaultOutputPath = "c:\\src\\tmp\\";

        /// <summary>
        /// Path where templates are located
        /// </summary>
        public string TemplatePath { get; set; }
        /// <summary>
        /// Name of template resource
        /// </summary>
        public string TemplateResource { get; set; }
        /// <summary>
        /// Schema name key 
        /// </summary>
        public string SchemaName { get; set; }
        /// <summary>
        /// Output path
        /// </summary>
        public string OutputPath { get; set; }
        /// <summary>
        /// Table list
        /// </summary>
        public List<string> TableList { get; set; }
        /// <summary>
        /// Company
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// Area name to use inside template files or in *Vars* / *Configurations* / *FinalVars* specification
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// Module name to use inside template files or in *Vars* / *Configurations* / *FinalVars* specification
        /// </summary>
        public string Module { get; set; }
        /// <summary>
        /// User variables
        /// </summary>
        public string UserVariables { get; set; }
        /// <summary>
        /// Generate referenced tables?
        /// </summary>
        public bool GenerateReferenced { get; set; }
        /// <summary>
        /// Remove folders?
        /// </summary>
        public bool RemoveFolders { get; set; }
        /// <summary>
        /// Created from schema name
        /// </summary>
        public string CreatedFromSchemaName { get; set; }
        /// <summary>
        /// Clean end of code line?
        /// </summary>
        public bool CleanEndOfCodeLine { get; set; } = true;
        /// <summary>
        /// Last pass?
        /// </summary>
        public bool LastPass { get; set; }
        /// <summary>
        /// Generator applicaton name
        /// </summary>
        public string GeneratorApplication { get; set; }
        /// <summary>
        /// Generator version
        /// </summary>
        public string GeneratorVersion { get; set; }
        /// <summary>
        /// Generate json info?
        /// </summary>
        public bool GenerateJsonInfo { get; set; }
        /// <summary>
        /// Path on json info will be generated
        /// </summary>
        public string JsonGeneratedPath { get; set; }
        /// <summary>
        /// Generate only json?
        /// </summary>
        public bool GeneateOnlyJson { get; set; }
        /// <summary>
        /// Output encoding
        /// </summary>
        public string Encoding { get; set; }
        /// <summary>
        /// End of line chars
        /// </summary>
        public string EndOfLine { get; set; }
        /// <summary>
        /// Schemas configuration
        /// </summary>
        public IBasicConfiguration<SchemaConfiguration> SchemasConfiguration { get; set; }
        public string ConnectionString { get; set; }
        public StringBuilderWriter SummaryWriter { get; set; }
        public string Template { get; set; }
    }
}
