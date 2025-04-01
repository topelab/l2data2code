using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.Schema
{
    public class NameResolver : INameResolver
    {
        private Dictionary<string, string> tableNames = null;
        private Dictionary<string, string> columnNames = null;
        private Dictionary<string, string> tableTypes = null;
        private Dictionary<string, string> enumTables = null;
        private Dictionary<string, string> descriptionTables = null;
        private Dictionary<string, Dictionary<string, ColumnFilter>> bigTables = null;
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
            descriptionTables = GetRenames(schemas[schemaName]?.DescriptionTables);
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
            => GetDescriptionsPairs(originalTableName, enumTables);

        public (string id, string name) ResolveDescriptionTables(string originalTableName)
            => GetDescriptionsPairs(originalTableName, descriptionTables);

        public bool IsWeakEntity(string originalTableName) => weakEntities.Contains(originalTableName);

        public bool IsBigTable(string originalTableName) => bigTables.ContainsKey(originalTableName);

        public Dictionary<string, ColumnFilter> GetBigTableColumns(string originalTableName) => bigTables.TryGetValue(originalTableName, out var value) ? value : [];

        private (string id, string name) GetDescriptionsPairs(string originalTableName, Dictionary<string, string> descriptions)
        {
            (string id, string name) result = (null, null);
            if (descriptions.TryGetValue(originalTableName, out var value) && value.Contains(','))
            {
                var colums = value.Split(',');
                result.id = colums[0];
                result.name = colums[1];
            }
            return result;
        }

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

        private static Dictionary<string, Dictionary<string, ColumnFilter>> GetBigTables(List<BigTable> bigTables)
        {
            Dictionary<string, Dictionary<string, ColumnFilter>> result = new();
            if (bigTables != null)
            {
                foreach (var bigTable in bigTables)
                {
                    var columnsFilter = ExtractColumnFilter(bigTable.ColumnsFilter);
                    result.Add(bigTable.Key, columnsFilter);
                }
            }
            return result;
        }

        private static Dictionary<string, ColumnFilter> ExtractColumnFilter(List<string> columnsFilter)
        {
            Dictionary<string, ColumnFilter> result = [];
            if (columnsFilter != null)
            {
                foreach (var columnFilter in columnsFilter)
                {
                    var columnName = columnFilter.Piece(0, ':');
                    var filterType = columnFilter.Piece(1, ':');
                    var filterSpecification = columnFilter.Piece(2, ':');
                    result.Add(columnName, new ColumnFilter { ColumnName = columnName, FilterType = filterType, FilterSpecification = filterSpecification });
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
