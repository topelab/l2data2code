using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SharedLib.Configuration;
using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.Schema
{
    public class NameResolver : INameResolver
    {
        private Dictionary<string, string> tableNames = null;
        private Dictionary<string, string> columnNames = null;
        private Dictionary<string , string> tableTypes = null;
        private Dictionary<string , string> enumTables = null;
        private Dictionary<string , List<string>> bigTables = null;
        private readonly List<string> weakEntities = [];

        private readonly IBasicConfiguration<SchemaConfiguration> schemas;

        public NameResolver(IBasicConfiguration<SchemaConfiguration> schemas)
        {
            this.schemas = schemas;
        }

        public void Initialize(string schemaName)
        {
            tableNames = GetRenames(schemas[schemaName]?.RenameTables);
            columnNames = GetRenames(schemas[schemaName]?.RenameColumns);
            tableTypes = GetRenames(schemas[schemaName]?.TableTypes);
            enumTables = GetRenames(schemas[schemaName]?.EnumTables);
            bigTables = GetBigTables(schemas[schemaName]?.BigTables);
            weakEntities.AddRange(GetSemiColonEntries(schemas[schemaName]?.WeakEntities));
        }

        public string ResolveTableName(string originalTableName) =>
            tableNames.TryGetValue(originalTableName, out var value) ? value : originalTableName;

        public string ResolveColumnName(string originalTableName, string originalColumnName)
        {
            var key = $"{originalTableName}.{originalColumnName}";
            return columnNames.TryGetValue(key, out var value) ? value : columnNames.TryGetValue(originalColumnName, out var value1) ? value1 : originalColumnName;
        }

        public string ResolveTableType(string originalTableName) =>
            tableTypes.TryGetValue(originalTableName, out var value) ? value : string.Empty;

        public (string id, string name) ResolveEnumTables(string originalTableName)
        {
            (string id, string name) result = (null, null);
            if (enumTables.TryGetValue(originalTableName, out var value) && value.Contains(','))
            {
                var colums = value.Split(',');
                result.id = colums[0];
                result.name = colums[1];
            }
            return result;
        }

        public bool IsWeakEntity(string originalTableName) => weakEntities.Contains(originalTableName);

        public bool IsBigTable(string originalTableName) => bigTables.ContainsKey(originalTableName);

        public List<string> GetBigTableColumns(string originalTableName) => bigTables.TryGetValue(originalTableName, out var value) ? value : [];

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

        private static Dictionary<string, List<string>> GetBigTables(List<BigTable> bigTables)
        {
            Dictionary<string, List<string>> result = new();
            if (bigTables != null)
            {
                foreach (var bigTable in bigTables)
                {
                    result.Add(bigTable.Key, bigTable.ColumnsFilter);
                }
            }   
            return result;
        }

        private static List<string> GetSemiColonEntries(string semiColonEntries)
        {
            List<string> entities = [];
            if (semiColonEntries != null)
            {
                entities.AddRange(semiColonEntries.Split(';', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries));
            }
            return entities;
        }

    }
}
