using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace L2Data2Code.SchemaReader.NpgSql
{
    public class NpgSchemaReader : Schema.SchemaReader
    {
        private readonly string connectionString;
        private readonly INameResolver nameResolver;
        private NpgsqlConnection connection;
        private readonly string defaultDBSchema = "public";

        public NpgSchemaReader(INameResolver nameResolver, ISchemaOptions options) : base(options.SummaryWriter)
        {
            connectionString = options.ConnectionString;
            this.nameResolver = nameResolver ?? throw new ArgumentNullException(nameof(nameResolver));
            this.nameResolver.Initialize(options.SchemaName);
        }

        public override Tables ReadSchema(SchemaReaderOptions options)
        {
            Tables result = [];

            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string[] schema = [connection.Database, defaultDBSchema, null, null];

                var tables = connection.GetSchema("Tables", schema);
                AddItems(options.TableRegex, options.AlternativeDescriptions, result, tables, false);

                var views = connection.GetSchema("Views", schema);
                AddItems(options.TableRegex, options.AlternativeDescriptions, result, views, true);

                try
                {
                    foreach (var tbl in result.Values)
                    {
                        tbl.Columns = LoadColumns(tbl, options.RemoveFirstWord, options.AlternativeDescriptions);

                        // Mark the primary key
                        var PrimaryKey = GetPK(tbl.Name);

                        foreach (var col in tbl.Columns)
                        {
                            if (PrimaryKey.ContainsKey(col.Name))
                            {
                                col.IsPK = true;
                                col.PkOrder = PrimaryKey[col.Name];
                            }
                        }

                        tbl.Indexes = GetIndexes(tbl.Name);
                        tbl.EnumValues = GetEnumValues(tbl);
                    }
                }
                catch (Exception x)
                {
                    HasErrorMessage(true);
                    var error = x.Message.ReplaceEndOfLine();
                    WriteLine("");
                    WriteLine("// -----------------------------------------------------------------------------------------");
                    WriteLine(String.Format("// Failed to get Columns - {0}", error));
                    WriteLine("// -----------------------------------------------------------------------------------------");
                    WriteLine("");
                }

                try
                {
                    var relations = LoadRelations(result);
                    foreach (var tbl in result.Values)
                    {
                        tbl.OuterKeys = relations.Where(r => r.ColumnReferencing.Table.Name.Equals(tbl.Name)).ToList();
                        tbl.InnerKeys = relations.Where(r => r.ColumnReferenced.Table.Name.Equals(tbl.Name)).ToList();

                    }
                }
                catch (Exception x)
                {
                    HasErrorMessage(true);
                    var error = x.Message.ReplaceEndOfLine();
                    WriteLine("");
                    WriteLine("// -----------------------------------------------------------------------------------------");
                    WriteLine(String.Format("// Failed to get relationships - {0}", error));
                    WriteLine("// -----------------------------------------------------------------------------------------");
                    WriteLine("");
                }
                connection.Close();

            }

            return result;
        }

        private List<EnumTableValue> GetEnumValues(Table tbl)
        {
            List<EnumTableValue> values = new List<EnumTableValue>();
            if (tbl.IsEnum)
            {
                var sql = $"select t.{tbl.EnumValue}, t.{tbl.EnumName} from {tbl.Name} t";
                using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    values.Add(new EnumTableValue { Id = rdr.GetInt32(0), Name = rdr.GetString(1) });
                }
            }
            return values;
        }

        private void AddItems(Regex tableRegex, Dictionary<string, string> alternativeDescriptions, Tables result, DataTable tables, bool fromViews)
        {
            foreach (DataRow row in tables.Rows)
            {
                if ((string)row["table_schema"] != connection.Database)
                {
                    continue;
                }

                Table tbl = new()
                {
                    Name = (string)row["table_name"],
                    SourceDB = "postgresql",
                };

                if (tableRegex != null && !tableRegex.IsMatch(tbl.Name))
                {
                    continue;
                }

                tbl.Schema = (string)row["table_schema"];
                tbl.IsView = fromViews;
                tbl.IsUpdatable = !fromViews || (string)row["is_updatable"] == "YES";
                tbl.CleanName = RemoveTablePrefixes(nameResolver.ResolveTableName(tbl.Name)).PascalCamelCase(false);
                tbl.Type = nameResolver.ResolveTableType(tbl.Name);
                (tbl.EnumValue, tbl.EnumName) = nameResolver.ResolveEnumTables(tbl.Name);
                tbl.ClassName = tbl.CleanName.ToSingular();
                tbl.Description = alternativeDescriptions != null && alternativeDescriptions.TryGetValue(tbl.Name, out var value) ? value : string.Empty;

                result.Add(tbl.Name, tbl);
            }
        }

        private List<Column> LoadColumns(Table tbl, bool removeFirstWord = true, Dictionary<string, string> alternativeDescriptions = null)
        {
            List<Column> result = [];

            string[] schema = [connection.Database, defaultDBSchema, tbl.Name, null];

            // Columnas
            var SchemaTabla = connection.GetSchema("Columns", schema);
            foreach (DataRow row in SchemaTabla.Rows)
            {
                Column col = new()
                {
                    Table = tbl,
                    TableName = tbl.Name,
                    Owner = null,
                    Name = (string)row["column_name"],
                    Precision = (int)row["character_maximum_length"].IfNull(row["numeric_precision"].IfNull(row["datetime_precision"].IfNull((ulong)0)))
                };
                col.PropertyName = nameResolver.ResolveColumnName(tbl.Name, col.Name).PascalCamelCase(removeFirstWord);
                col.PropertyType = GetPropertyType((string)row["data_type"], col.Precision);
                col.Precision = col.PropertyType == "bool" ? 1 : col.Precision;
                col.IsNullable = ((string)row["is_nullable"]) == "YES";
                col.NumericScale = (int)row["numeric_scale"].IfNull((ulong)0);
                col.IsNumeric = row["numeric_precision"].IfNull(0) > 0;
                col.IsComputed = tbl.IsView;
                col.DefaultValue = row["column_default"].IfNull<string>(null) == null ? null : ((string)row["column_default"]).RemoveOuter('(', ')').RemoveOuter('\'').Replace("current_timestamp", "DateTime.Now", StringComparison.CurrentCultureIgnoreCase);
                col.Description = alternativeDescriptions != null && alternativeDescriptions.TryGetValue(tbl.Name, out var value) ? value : (string)row["column_comment"];
                if (col.DefaultValue != null && col.PropertyType == "decimal" && !col.DefaultValue.EndsWith("m") && col.DefaultValue.Contains('.'))
                {
                    col.DefaultValue += "m";
                }
                result.Add(col);
            }

            return result;
        }

        private List<Key> LoadRelations(Tables tables)
        {
            List<Key> result = [];
            using var cmd = connection.CreateCommand();
            cmd.CommandText = ALL_FOREIGN_KEYS;
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Key key = new();
                var referencingTable = reader["table_name"].ToString();
                var referencingColumn = reader["column_name"].ToString();
                var referencedTable = reader["referenced_table_name"].ToString();
                var referencedColumn = reader["referenced_column_name"].ToString();

                WriteLine($"// referencedColumn: {referencedColumn}, referencingColumn: {referencingColumn}");

                var tableReferencing = tables[referencingTable];
                var tableReferenced = tables[referencedTable];

                key.Name = reader["constraint_name"].ToString();
                key.ColumnReferencing = tableReferencing.GetColumn(referencingColumn);
                key.ColumnReferenced = tableReferenced.GetColumn(referencedColumn);

                result.Add(key);
            }

            return result;
        }

        private Dictionary<string, int> GetPK(string table)
        {

            Dictionary<string, int> result = new();

            var databaseIndexColumns = connection.GetSchema("IndexColumns", [connection.Database, defaultDBSchema, table, null]);

            foreach (DataRow row in databaseIndexColumns.Rows)
            {
                if (row["index_name"].ToString().Equals("primary"))
                {
                    result.Add(row["column_name"].ToString(), Convert.ToInt32(row["ordinal_position"]));
                }
            }

            return result;
        }

        private List<Schema.Index> GetIndexes(string table)
        {
            List<Schema.Index> result = [];

            var databaseIndexes = connection.GetSchema("Indexes", [connection.Database, defaultDBSchema, table, null]).Rows.Cast<DataRow>();
            var databaseIndexColumns = connection.GetSchema("IndexColumns", [connection.Database, defaultDBSchema, table, null]).Rows.Cast<DataRow>();

            databaseIndexes.Where(r => !(bool)r["primary"])
                .Select(r => new { IndexName = (string)r["index_name"], IsUnique = (bool)r["unique"] })
                .ToList()
                .ForEach(i =>
                {
                    var columns = databaseIndexColumns.Where(r => (string)r["index_name"] == i.IndexName)
                            .Select(r => new IndexColumn((string)r["column_name"], Convert.ToInt32(r["ordinal_position"]), (string)r["sort_order"] != "A"))
                            .ToList();

                    result.Add(new Schema.Index(i.IndexName, i.IsUnique, columns));
                });

            return result;
        }

        private static string GetPropertyType(string sqlType, int precision = 0, string dbTypeOriginal = null)
        {
            var sysType = "string";
            switch (sqlType)
            {
                case "blob":
                case "binary":
                case "mediumblob":
                case "longblob":
                case "varbinary":
                    sysType = "byte[]";
                    break;
                case "bit":
                case "bool":
                case "boolean":
                    sysType = "bool";
                    break;
                case "char":
                    sysType = "char";
                    break;
                case "date":
                case "datetime":
                case "timestamp":
                    sysType = "DateTime";
                    break;
                case "time":
                    sysType = "TimeSpan";
                    break;
                case "float":
                case "decimal":
                    sysType = "decimal";
                    break;
                case "double":
                    sysType = "double";
                    break;
                case "bigint":
                    sysType = "long";
                    break;
                case "smallint":
                case "int":
                case "mediumint":
                    if (precision < 12)
                    {
                        sysType = "int";
                    }
                    else
                    {
                        sysType = "long";
                    }
                    break;
                case "tinyint":
                    sysType = precision == 1 || dbTypeOriginal == "tinyint(1)" ? "bool" : "sbyte";
                    break;

            }
            return sysType;
        }

        private static string RemoveTablePrefixes(string word)
        {
            var cleanword = word;
            if (cleanword.StartsWith("tbl_"))
            {
                cleanword = cleanword.Replace("tbl_", "");
            }

            if (cleanword.StartsWith("tbl"))
            {
                cleanword = cleanword.Replace("tbl", "");
            }

            return cleanword;
        }

        private const string ALL_FOREIGN_KEYS = """
            SELECT
                tc.table_schema, 
                tc.constraint_name, 
                tc.table_name, 
                kcu.column_name, 
                ccu.table_schema AS referenced_table_schema,
                ccu.table_name AS referenced_table_name,
                ccu.column_name AS referenced_column_name 
            FROM information_schema.table_constraints AS tc 
            JOIN information_schema.key_column_usage AS kcu
                ON tc.constraint_name = kcu.constraint_name
                AND tc.table_schema = kcu.table_schema
            JOIN information_schema.constraint_column_usage AS ccu
                ON ccu.constraint_name = tc.constraint_name
            WHERE tc.constraint_type = 'FOREIGN KEY'
                AND tc.table_schema='public'; 
            """;

    }
}
