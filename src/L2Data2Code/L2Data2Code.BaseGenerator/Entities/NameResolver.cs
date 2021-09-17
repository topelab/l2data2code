using L2Data2Code.SchemaReader.Interface;
using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Entities
{
    public class NameResolver : INameResolver
    {
        private Dictionary<string, string> _tableNames = null;
        private Dictionary<string, string> _columnNames = null;

        public NameResolver(string schemaName)
        {
            _tableNames = Config.GetTableRenames(schemaName);
            _columnNames = Config.GetColumnRenames(schemaName);
        }

        public string ResolveTableName(string originalTableName) =>
            _tableNames.ContainsKey(originalTableName) ? _tableNames[originalTableName] : originalTableName;

        public string ResolveColumnName(string originalTableName, string originalColumnName)
        {
            string key = $"{originalTableName}.{originalColumnName}";
            return _columnNames.ContainsKey(key) ? _columnNames[key] : (_columnNames.ContainsKey(originalColumnName) ? _columnNames[originalColumnName] : originalColumnName);
        }
    }
}
