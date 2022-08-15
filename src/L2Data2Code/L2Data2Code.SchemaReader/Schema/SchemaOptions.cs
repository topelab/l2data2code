using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SharedLib.Configuration;

namespace L2Data2Code.SchemaReader.Schema
{
    public class SchemaOptions : ISchemaOptions
    {
        public IBasicConfiguration<SchemaConfiguration> SchemasConfiguration { get; set; }
        public string SchemaName { get; set; }
        public string ConnectionString { get; set; }
        public StringBuilderWriter SummaryWriter { get; set; }
        public string TemplatePath { get; set; }
        public string CreatedFromSchemaName { get; set; }
    }
}
