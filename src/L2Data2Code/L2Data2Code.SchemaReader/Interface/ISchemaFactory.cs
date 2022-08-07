using L2Data2Code.SchemaReader.Schema;

namespace L2Data2Code.SchemaReader.Interface
{
    public interface ISchemaFactory
    {
        ISchemaReader Create(ISchemaOptions schemaOptions);
        string GetConversion(string provider, string type);
        string GetProviderDefinitionKey(string connectionStringKey);
    }
}