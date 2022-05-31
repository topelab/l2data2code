using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Configuration;

namespace L2Data2Code.BaseGenerator.Interfaces
{
    public interface ISchemaOptionsFactory
    {
        SchemaOptions Create(CodeGeneratorDto codeGeneratorDto, StringBuilderWriter stringBuilderWriter);
        SchemaOptions Create(string templatePath, IBasicConfiguration<SchemaConfiguration> schemasConfiguration, string schemaName, StringBuilderWriter summaryWriter, string descriptionsSchemaName);
    }
}