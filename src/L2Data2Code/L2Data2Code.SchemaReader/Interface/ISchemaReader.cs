using L2Data2Code.SchemaReader.Schema;

namespace L2Data2Code.SchemaReader.Interface
{
    public interface ISchemaReader
    {
        Tables ReadSchema(SchemaReaderOptions schemaReaderOptions);
        void WriteLine(string msg);
        void HasErrorMessage(bool setError);
        bool HasErrorMessage();
        bool CanConnect();
    }
}
