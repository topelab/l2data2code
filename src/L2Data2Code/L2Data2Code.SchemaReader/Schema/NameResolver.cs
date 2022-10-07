using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SharedLib.Configuration;
using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.Schema
{
    public class NameResolver : INameResolver
    {
        private Dictionary<string, string> _tableNames = null;
        private Dictionary<string, string> _columnNames = null;
        private Dictionary<string , string> _tableTypes = null;
        private readonly IBasicConfiguration<SchemaConfiguration> schemas;

        public NameResolver(IBasicConfiguration<SchemaConfiguration> schemas)
        {
            this.schemas = schemas;
        }

        public void Initialize(string schemaName)
        {
            _tableNames = GetRenames(schemas[schemaName]?.RenameTables);
            _columnNames = GetRenames(schemas[schemaName]?.RenameColumns);
            _tableTypes = GetRenames(schemas[schemaName]?.TableTypes);
        }

        public string ResolveTableName(string originalTableName) =>
            _tableNames.ContainsKey(originalTableName) ? _tableNames[originalTableName] : originalTableName;

        public string ResolveColumnName(string originalTableName, string originalColumnName)
        {
            var key = $"{originalTableName}.{originalColumnName}";
            return _columnNames.ContainsKey(key) ? _columnNames[key] : _columnNames.ContainsKey(originalColumnName) ? _columnNames[originalColumnName] : originalColumnName;
        }

        public string ResolveTableType(string originalTableName) =>
            _tableTypes.ContainsKey(originalTableName) ? _tableTypes[originalTableName] : string.Empty;

        private static Dictionary<string, string> GetRenames(string renameDescriptions)
        {
            Dictionary<string, string> renames = new();
            if (renameDescriptions != null)
            {
                foreach (var item in renameDescriptions.Split(';'))
                {
                    var def = item.Split('=');
                    renames.Add(def[0].Trim(), def[1].Trim());
                }
            }
            return renames;
        }

    }
}
