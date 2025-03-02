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
        private Dictionary<string , string> _enumTables = null;
        private readonly List<string> _weakEntities = [];

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
            _enumTables = GetRenames(schemas[schemaName]?.EnumTables);
            var weakEntities = schemas[schemaName]?.WeakEntities;
            if (weakEntities != null)
            {
                _weakEntities.AddRange(weakEntities.Split(';'));
            }
        }

        public string ResolveTableName(string originalTableName) =>
            _tableNames.TryGetValue(originalTableName, out var value) ? value : originalTableName;

        public string ResolveColumnName(string originalTableName, string originalColumnName)
        {
            var key = $"{originalTableName}.{originalColumnName}";
            return _columnNames.TryGetValue(key, out var value) ? value : _columnNames.TryGetValue(originalColumnName, out var value1) ? value1 : originalColumnName;
        }

        public string ResolveTableType(string originalTableName) =>
            _tableTypes.TryGetValue(originalTableName, out var value) ? value : string.Empty;

        public (string id, string name) ResolveEnumTables(string originalTableName)
        {
            (string id, string name) result = (null, null);
            if (_enumTables.TryGetValue(originalTableName, out var value) && value.Contains(','))
            {
                var colums = value.Split(',');
                result.id = colums[0];
                result.name = colums[1];
            }
            return result;
        }

        public bool IsWeakEntity(string originalTableName) => _weakEntities.Contains(originalTableName);

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
