using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace L2Data2Code.SchemaReader.MySql
{
    public class MySqlSchemaReader : Schema.SchemaReader
    {
        private readonly string connectionString;
        private MySqlConnection connection;
        private readonly INameResolver nameResolver;


        public MySqlSchemaReader(INameResolver nameResolver, ISchemaOptions options) : base(options.SummaryWriter)
        {
            connectionString = options.ConnectionString;
            this.nameResolver = nameResolver ?? throw new ArgumentNullException(nameof(nameResolver));
            this.nameResolver.Initialize(options.SchemaName);
        }

        public override Tables ReadSchema(SchemaReaderOptions options)
        {
            Tables result = new();

            using (connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                var schema = new string[4] { null, connection.Database, null, null };

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

        private void AddItems(Regex tableRegex, Dictionary<string, string> alternativeDescriptions, Tables result, DataTable tables, bool fromViews)
        {
            foreach (DataRow row in tables.Rows)
            {
                if ((string)row["TABLE_SCHEMA"] != connection.Database)
                {
                    continue;
                }

                Table tbl = new()
                {
                    Name = (string)row["TABLE_NAME"]
                };

                if (tableRegex != null && !tableRegex.IsMatch(tbl.Name))
                {
                    continue;
                }

                tbl.Schema = (string)row["TABLE_SCHEMA"];
                tbl.IsView = fromViews;
                tbl.IsUpdatable = !fromViews || (string)row["IS_UPDATABLE"] == "YES";
                tbl.CleanName = RemoveTablePrefixes(nameResolver.ResolveTableName(tbl.Name)).PascalCamelCase(false);
                tbl.ClassName = tbl.CleanName.ToSingular();
                tbl.Description = alternativeDescriptions != null && alternativeDescriptions.ContainsKey(tbl.Name)
                    ? alternativeDescriptions[tbl.Name]
                    : (fromViews ? string.Empty : (string)row["TABLE_COMMENT"]);

                result.Add(tbl.Name, tbl);
            }
        }

        private List<Column> LoadColumns(Table tbl, bool removeFirstWord = true, Dictionary<string, string> alternativeDescriptions = null)
        {
            List<Column> result = new();

            var schema = new string[4] { null, connection.Database, tbl.Name, null };

            // Columnas
            var SchemaTabla = connection.GetSchema("Columns", schema);
            foreach (DataRow row in SchemaTabla.Rows)
            {
                Column col = new()
                {
                    Table = tbl,
                    TableName = tbl.Name,
                    Owner = null,
                    Name = (string)row["COLUMN_NAME"],
                    Precision = (int)row["CHARACTER_MAXIMUM_LENGTH"].IfNull(row["NUMERIC_PRECISION"].IfNull(row["DATETIME_PRECISION"].IfNull((ulong)0)))
                };
                col.PropertyName = nameResolver.ResolveColumnName(tbl.Name, col.Name).PascalCamelCase(removeFirstWord);
                col.PropertyType = GetPropertyType((string)row["DATA_TYPE"], col.Precision, (string)row["COLUMN_TYPE"]);
                col.IsNullable = ((string)row["IS_NULLABLE"]) == "YES";
                col.IsAutoIncrement = ((string)row["EXTRA"]) == "auto_increment";
                col.NumericScale = (int)row["NUMERIC_SCALE"].IfNull((ulong)0);
                col.IsNumeric = row["NUMERIC_PRECISION"].IfNull(0) > 0;
                col.IsComputed = tbl.IsView || !string.IsNullOrWhiteSpace((string)row["EXTRA"]);
                col.Description = alternativeDescriptions != null && alternativeDescriptions.ContainsKey(tbl.Name) ? alternativeDescriptions[col.Name] : (string)row["COLUMN_COMMENT"];
                result.Add(col);
            }

            return result;
        }

        private List<Key> LoadRelations(Tables tables)
        {
            var SchemaName = connection.Database;
            var schema = new string[4] { null, SchemaName, null, null };
            List<Key> result = new();

            // Relaciones
            var fkeys = connection.GetSchema("Foreign Key Columns", schema);

            foreach (DataRow row in fkeys.Rows)
            {
                Key key = new();
                var referencingTable = row["TABLE_NAME"].ToString();
                var referencingColumn = row["COLUMN_NAME"].ToString();
                var referencedTable = row["REFERENCED_TABLE_NAME"].ToString();
                var referencedColumn = row["REFERENCED_COLUMN_NAME"].ToString();

                WriteLine($"// referencedColumn: {referencedColumn}, referencingColumn: {referencingColumn}");

                var tableReferencing = tables[referencingTable];
                var tableReferenced = tables[referencedTable];

                key.Name = row["CONSTRAINT_NAME"].ToString();
                key.ColumnReferencing = tableReferencing.GetColumn(referencingColumn);
                key.ColumnReferenced = tableReferenced.GetColumn(referencedColumn);

                result.Add(key);

            }

            return result;
        }

        private Dictionary<string, int> GetPK(string table)
        {

            Dictionary<string, int> result = new();

            var databaseIndexColumns = connection.GetSchema("IndexColumns", new string[4] { null, connection.Database, table, null });

            foreach (DataRow row in databaseIndexColumns.Rows)
            {
                if (row["INDEX_NAME"].ToString().Equals("PRIMARY"))
                {
                    result.Add(row["COLUMN_NAME"].ToString(), Convert.ToInt32(row["ORDINAL_POSITION"]));
                }
            }

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

    }

}
