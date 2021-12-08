using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Configuration;

namespace L2Data2Code.BaseGenerator.Services
{
    public class SchemaOptionsFactory : ISchemaOptionsFactory
    {
        public SchemaOptions Create(CodeGeneratorDto codeGeneratorDto, StringBuilderWriter stringBuilderWriter)
        {
            return new SchemaOptions
            {
                SchemasConfiguration = codeGeneratorDto.SchemasConfiguration,
                SchemaName = codeGeneratorDto.SchemaName,
                SummaryWriter = stringBuilderWriter,
                DescriptionsSchemaName = codeGeneratorDto.DescriptionsSchemaName
            };
        }

        public SchemaOptions Create(IBasicConfiguration<SchemaConfiguration> schemasConfiguration, string schemaName, StringBuilderWriter summaryWriter, string descriptionsSchemaName)
        {
            return new SchemaOptions
            {
                SchemasConfiguration = schemasConfiguration,
                SchemaName = schemaName,
                SummaryWriter = summaryWriter,
                DescriptionsSchemaName = descriptionsSchemaName
            };
        }
    }
}
