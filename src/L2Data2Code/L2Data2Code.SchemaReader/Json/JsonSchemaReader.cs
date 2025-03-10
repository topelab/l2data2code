using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace L2Data2Code.SchemaReader.Json
{
    public class JsonSchemaReader : Schema.SchemaReader
    {
        private readonly string connectionString;
        private readonly string templatePath;
        private readonly INameResolver nameResolver;

        public JsonSchemaReader(INameResolver nameResolver, ISchemaOptions options) : base(options.SummaryWriter)
        {

            connectionString = options.ConnectionString;
            templatePath = options.TemplatePath;
            var fileExist = templatePath.GetResultUsingBasePath(() => File.Exists(connectionString));
            if (!fileExist)
            {
                throw new Exception($"JSON file {connectionString} doesn't exist");
            }
            this.nameResolver = nameResolver ?? throw new ArgumentNullException(nameof(nameResolver));
            this.nameResolver.Initialize(options.SchemaName);
        }

        public override Tables ReadSchema(SchemaReaderOptions options)
        {
            var content = templatePath.GetResultUsingBasePath(() => File.ReadAllText(connectionString));
            List<Table> tableList = new();

            if (content.StartsWith("["))
            {
                tableList.AddRange(JsonConvert.DeserializeObject<List<Table>>(content));
            }
            else
            {
                tableList.AddRange(JsonConvert.DeserializeObject<TablesDTO>(content).Tables);
            }
            return Resolve(tableList, options.RemoveFirstWord, options.TableRegex);
        }

        /// <summary>
        /// Resolve <see cref="Table.InnerKeys" /> and <see cref="Table.OuterKeys" /> for the specified tables.
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <returns></returns>
        private Tables Resolve(IEnumerable<Table> tables, bool removeFirstWord, Regex tableRegex = null)
        {
            Tables result = new();
            var tablesInfo = tables.ToDictionary(k => k.Name.ToUpper(), k => k);
            foreach (var item in tables.Where(t => tableRegex == null || tableRegex.IsMatch(t.Name)))
            {
                item.CleanName = nameResolver.ResolveTableName(item.Name).PascalCamelCase(false);
                item.Type = nameResolver.ResolveTableType(item.Name);
                item.ClassName = item.CleanName.ToSingular();
                item.IsWeakEntity = nameResolver.IsWeakEntity(item.Name);
                item.IsBigTable = nameResolver.IsBigTable(item.Name);
                var filteredColumns = nameResolver.GetBigTableColumns(item.Name);
                foreach (var column in item.Columns)
                {
                    column.Table = item;
                    column.PropertyName = column.PropertyName.IsEmpty() ? nameResolver.ResolveColumnName(item.Name, column.Name).PascalCamelCase(removeFirstWord) : column.PropertyName;
                    if (column.DefaultValue != null && column.PropertyType == "decimal" && !column.DefaultValue.EndsWith("m") && column.DefaultValue.Contains('.'))
                    {
                        column.DefaultValue += "m";
                    }
                    column.IsFilter = filteredColumns.Contains(column.Name);
                }
                ResolveKeys(item.InnerKeys, tablesInfo);
                ResolveKeys(item.OuterKeys, tablesInfo);
                result.Add(item.Name, item);
            }
            return result;
        }

        /// <summary>
        /// Resolves the keys.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="tables">The tables.</param>
        private static void ResolveKeys(IEnumerable<Key> keys, Dictionary<string, Table> tables)
        {
            foreach (var item in keys)
            {
                item.ColumnReferenced = GetColum(tables, item.Referenced);
                item.ColumnReferencing = GetColum(tables, item.Referencing);
            }
        }

        /// <summary>
        /// Search <paramref name="reference"/> (format: table.column) and return a <see cref="Column"/> or null if not found.
        /// </summary>
        /// <param name="tables">The tables to search.</param>
        /// <param name="reference">The table.column string reference.</param>
        /// <returns></returns>
        private static Column GetColum(Dictionary<string, Table> tables, string reference)
        {
            if (reference.IsEmpty() || !reference.Contains('.'))
            {
                return null;
            }

            var referenceParts = reference.Split(".");
            var tableName = referenceParts[0].ToUpper();
            var columnName = referenceParts[1].ToUpper();
            if (!tables.ContainsKey(tableName))
            {
                return null;
            }

            var table = tables[tableName];
            var findedColumn = table.Columns.FirstOrDefault(c => c.Name.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
            return findedColumn;
        }


    }
}
