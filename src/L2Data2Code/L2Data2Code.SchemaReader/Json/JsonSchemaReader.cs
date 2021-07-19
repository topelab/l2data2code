using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace L2Data2Code.SchemaReader.Json
{
    public class JsonSchemaReader : Schema.SchemaReader
    {
        private readonly string _connectionString;
        private INameResolver _resolver;

        public JsonSchemaReader(SchemaOptions options) : base(options.SummaryWriter)
        {
            _connectionString = options.ConnectionString;
            if (!File.Exists(_connectionString))
            {
                throw new Exception($"JSON file {_connectionString} doesn't exist");
            }
        }

        public override Tables ReadSchema(SchemaReaderOptions options)
        {
            _resolver = options.NameResolver ?? new DefaultNameResolver();

            var content = File.ReadAllText(_connectionString);
            var tableList = JsonConvert.DeserializeObject<List<Table>>(content);
            return Resolve(tableList, options.RemoveFirstWord, options.TableRegex);
        }

        /// <summary>
        /// Resolve <see cref="Table.InnerKeys" /> and <see cref="Table.OuterKeys" /> the specified tables.
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <returns></returns>
        private Tables Resolve(IEnumerable<Table> tables, bool removeFirstWord, Regex tableRegex = null)
        {
            var result = new Tables();
            var tablesInfo = tables.ToDictionary(k => k.Name.ToUpper(), k => k);
            foreach (var item in tables.Where(t => tableRegex == null || tableRegex.IsMatch(t.Name)))
            {
                item.CleanName = _resolver.ResolveTableName(item.Name).PascalCamelCase(false);
                item.ClassName = item.CleanName.ToSingular();
                foreach (var column in item.Columns)
                {
                    column.Table = item;
                    column.PropertyName = column.PropertyName.IsEmpty() ? _resolver.ResolveColumnName(item.Name, column.Name).PascalCamelCase(removeFirstWord) : column.PropertyName;
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
        private void ResolveKeys(IEnumerable<Key> keys, Dictionary<string, Table> tables)
        {
            foreach (var item in keys)
            {
                item.ColumnReferenced = GetColum(tables, item.Referenced);
                item.ColumnReferencing = GetColum(tables, item.Referencing);
            }
        }

        /// <summary>
        /// Search <paramref name="reference"/> (format: table.column) and return a <see cref="Column"/> or null if not finded.
        /// </summary>
        /// <param name="tables">The tables to search.</param>
        /// <param name="reference">The table.column string reference.</param>
        /// <returns></returns>
        private Column GetColum(Dictionary<string, Table> tables, string reference)
        {
            if (reference.IsEmpty() || !reference.Contains("."))
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
