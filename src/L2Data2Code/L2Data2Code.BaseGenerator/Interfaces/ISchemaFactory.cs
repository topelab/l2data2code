using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Schema;

namespace L2Data2Code.BaseGenerator.Interfaces
{
    public interface ISchemaFactory
    {
        ISchemaReader Create(SchemaOptions schemaOptions);
        string GetConversion(string provider, string type);
        string GetProviderDefinitionKey(string connectionStringKey);
    }
}