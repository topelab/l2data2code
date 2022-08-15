using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SharedLib.Configuration;

namespace L2Data2Code.SchemaReader.Interface
{
    public interface ISchemaOptions
    {
        string ConnectionString { get; set; }
        string SchemaName { get; set; }
        IBasicConfiguration<SchemaConfiguration> SchemasConfiguration { get; set; }
        StringBuilderWriter SummaryWriter { get; set; }
        string TemplatePath { get; set; }
        string CreatedFromSchemaName { get; set; }
    }
}