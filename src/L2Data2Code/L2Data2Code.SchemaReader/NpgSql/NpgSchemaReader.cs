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
        private readonly IForeignKeysGetter<NpgsqlConnection> foreignKeysGetter;
        private readonly IColumnsGetter<NpgsqlConnection> columnsGetter;
        private NpgsqlConnection connection;

        public  static readonly string DefaultDBSchema = "public";

        public NpgSchemaReader(INameResolver nameResolver,
                               ISchemaOptions options,
                               IForeignKeysGetter<NpgsqlConnection> foreignKeysGetter,
                               IColumnsGetter<NpgsqlConnection> columnsGetter) : base(options.SummaryWriter)
        {
            connectionString = options.ConnectionString;
            this.nameResolver = nameResolver ?? throw new ArgumentNullException(nameof(nameResolver));
            this.foreignKeysGetter = foreignKeysGetter ?? throw new ArgumentNullException(nameof(foreignKeysGetter));
            this.columnsGetter = columnsGetter ?? throw new ArgumentNullException(nameof(columnsGetter));

            this.nameResolver.Initialize(options.SchemaName);
        }

        public override Tables ReadSchema(SchemaReaderOptions options)
        {
            Tables result = [];

            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string[] schema = [connection.Database, DefaultDBSchema, null, null];

                var tables = connection.GetSchema("Tables", schema);
                AddItems(options.TableRegex, options.AlternativeDescriptions, result, tables, false);

                var views = connection.GetSchema("Views", schema);
                AddItems(options.TableRegex, options.AlternativeDescriptions, result, views, true);

                try
                {
                    foreach (var tbl in result.Values)
                    {
                        tbl.Columns = columnsGetter.GetColumns(connection, tbl, options, nameResolver);

                        var allIndexs = GetAllIndex();
                        var PrimaryKey = GetPK(tbl.Name, allIndexs);

                        foreach (var col in tbl.Columns)
                        {
                            if (PrimaryKey.ContainsKey(col.Name))
                            {
                                col.IsPK = true;
                                col.PkOrder = PrimaryKey[col.Name];
                            }
                        }

                        tbl.Indexes = GetIndexes(tbl.Name, allIndexs);
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
                    var relations = foreignKeysGetter.GetForeignKeys(connection, result);
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
                if ((string)row["table_catalog"] != connection.Database)
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

                tbl.Schema = (string)row["table_catalog"];
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

        private DataTable GetAllIndex()
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = ALL_INDEXS_COLUMNS;

            using var reader = cmd.ExecuteReader();

            DataTable allIndexes = new();
            allIndexes.Load(reader);
            return allIndexes;
        }

        private Dictionary<string, int> GetPK(string table, DataTable allIndexes)
        {
            Dictionary<string, int> result = new();

            foreach (DataRow row in allIndexes.Rows.Cast<DataRow>().Where(r => (string)r["table_name"] == table))
            {
                if (row["primary"].IfNull(false))
                {
                    result.Add(row["column_name"].ToString(), row["ordinal_position"].IfNull(0));
                }
            }

            return result;
        }

        private List<Schema.Index> GetIndexes(string table, DataTable allIndexes)
        {
            List<Schema.Index> result = [];

            var databaseIndexes = connection.GetSchema("Indexes", [connection.Database, DefaultDBSchema, table, null]).Rows.Cast<DataRow>();
            var databaseIndexColumns = connection.GetSchema("IndexColumns", [connection.Database, DefaultDBSchema, table, null]).Rows.Cast<DataRow>();

            databaseIndexes.Where(r => !(bool)r["primary"])
                .Select(r => new { IndexName = (string)r["index_name"], IsUnique = r["unique"].IfNull(false) })
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
