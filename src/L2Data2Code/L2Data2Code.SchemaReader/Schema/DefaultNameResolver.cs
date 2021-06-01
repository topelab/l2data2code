using L2Data2Code.SchemaReader.Interface;

namespace L2Data2Code.SchemaReader.Schema
{
    public class DefaultNameResolver : INameResolver
    {
        public string ResolveColumnName(string originalTableName, string originalColumnName) => originalColumnName;

        public string ResolveTableName(string originalTableName) => originalTableName;
    }
}
