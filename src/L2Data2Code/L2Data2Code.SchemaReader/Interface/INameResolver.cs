using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.Interface
{
    public interface INameResolver
    {
        void Initialize(string schemaName);
        string ResolveColumnName(string originalTableName, string originalColumnName);
        string ResolveTableName(string originalTableName);
        string ResolveTableType(string originalTableName);
        (string id, string name) ResolveEnumTables(string originalTableName);
        bool IsWeakEntity(string originalTableName);
        bool IsBigTable(string originalTableName);
        List<string> GetBigTableColumns(string originalTableName);
    }
}