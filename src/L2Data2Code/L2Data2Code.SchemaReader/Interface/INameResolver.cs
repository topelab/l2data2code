namespace L2Data2Code.SchemaReader.Interface
{
    public interface INameResolver
    {
        void Initialize(string schemaName);
        string ResolveColumnName(string originalTableName, string originalColumnName);
        string ResolveTableName(string originalTableName);
    }
}