using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SharedLib.Configuration;

namespace L2Data2Code.SchemaReader.Schema
{
    public class SchemaOptionsFactory : ISchemaOptionsFactory
    {
        public ISchemaOptions Create(string templatePath, IBasicConfiguration<SchemaConfiguration> schemasConfiguration, string schemaName, StringBuilderWriter summaryWriter)
        {
            return new SchemaOptions
            {
                SchemasConfiguration = schemasConfiguration,
                SchemaName = schemaName,
                SummaryWriter = summaryWriter,
                TemplatePath = templatePath,
                CreatedFromSchemaName = schemaName
            };
        }
    }
}
