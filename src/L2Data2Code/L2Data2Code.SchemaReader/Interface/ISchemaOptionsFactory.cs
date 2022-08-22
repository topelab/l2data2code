using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SharedLib.Configuration;

namespace L2Data2Code.SchemaReader.Interface
{
    public interface ISchemaOptionsFactory
    {
        ISchemaOptions Create(string templatePath, IBasicConfiguration<SchemaConfiguration> schemasConfiguration, string schemaName, StringBuilderWriter summaryWriter);
    }
}