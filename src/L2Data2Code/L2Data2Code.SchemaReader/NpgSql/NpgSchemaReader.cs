using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using MySqlX.XDevAPI.Common;
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
        private readonly IColumnDescriptionsGetter<NpgsqlConnection> columnDescriptionsGetter;
        private readonly IColumnIdentitiesGetter<NpgsqlConnection> columnIdentitiesGetter;
        private readonly IIndexesGetter<NpgsqlConnection> indexesGetter;
        private NpgsqlConnection connection;

        public  static readonly string DefaultDBSchema = "public";

        public NpgSchemaReader(INameResolver nameResolver,
                               ISchemaOptions options,
                               IForeignKeysGetter<NpgsqlConnection> foreignKeysGetter,
                               IColumnsGetter<NpgsqlConnection> columnsGetter,
                               IColumnDescriptionsGetter<NpgsqlConnection> columnDescriptionsGetter,
                               IColumnIdentitiesGetter<NpgsqlConnection> columnIdentitiesGetter,
                               IIndexesGetter<NpgsqlConnection> indexesGetter) : base(options.SummaryWriter)
        {
            connectionString = options.ConnectionString;
            this.nameResolver = nameResolver ?? throw new ArgumentNullException(nameof(nameResolver));
            this.foreignKeysGetter = foreignKeysGetter ?? throw new ArgumentNullException(nameof(foreignKeysGetter));
            this.columnsGetter = columnsGetter ?? throw new ArgumentNullException(nameof(columnsGetter));
            this.columnDescriptionsGetter = columnDescriptionsGetter ?? throw new ArgumentNullException(nameof(columnDescriptionsGetter));
            this.columnIdentitiesGetter = columnIdentitiesGetter ?? throw new ArgumentNullException(nameof(columnIdentitiesGetter));
            this.indexesGetter = indexesGetter ?? throw new ArgumentNullException(nameof(indexesGetter));

            this.nameResolver.Initialize(options.SchemaName);
        }

        public override bool CanConnect()
        {
            var result = false;
            try
            {
                using var testConnection = new NpgsqlConnection(connectionString);
                testConnection.Open();
                if (testConnection.State == ConnectionState.Open)
                {
                    testConnection.Close();
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
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

                var indexes = indexesGetter.GetIndexes(connection);
                
                try
                {
                    foreach (var tbl in result.Values)
                    {
                        tbl.Columns = columnsGetter.GetColumns(connection, tbl, options, nameResolver);

                        var PrimaryKey = GetPK(tbl.Name, indexes);
                        var filteredColumns = nameResolver.GetBigTableColumns(tbl.Name);

                        foreach (var col in tbl.Columns)
                        {
                            if (PrimaryKey.ContainsKey(col.Name))
                            {
                                col.IsPK = true;
                                col.PkOrder = PrimaryKey[col.Name];
                            }
                            TrySetFilterColumn(filteredColumns, col);
                        }

                        tbl.Indexes = indexes.Where(i => i.TableName == tbl.Name && !i.IsPrimary).ToList();
                        tbl.EnumValues = GetEnumValues(tbl);
                    }

                    columnDescriptionsGetter.SetColumnsDescriptions(connection, result);
                    columnIdentitiesGetter.SetColumnsIdentities(connection, result);
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
                var sql = $"select t.\"{tbl.EnumValue}\", t.\"{tbl.EnumName}\" from \"{tbl.Name}\" t";
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
                (tbl.DescriptionId, tbl.DescriptionColumn) = nameResolver.ResolveDescriptionTables(tbl.Name);
                tbl.ClassName = tbl.CleanName.ToSingular();
                tbl.Description = alternativeDescriptions != null && alternativeDescriptions.TryGetValue(tbl.Name, out var value) ? value : string.Empty;
                tbl.IsWeakEntity = nameResolver.IsWeakEntity(tbl.Name);
                tbl.IsBigTable = nameResolver.IsBigTable(tbl.Name);

                result.Add(tbl.Name, tbl);
            }
        }

        private Dictionary<string, int> GetPK(string table, List<Schema.Index> indexes)
        {
            Dictionary<string, int> result = [];
            var primaryKeys = indexes.FirstOrDefault(i => i.TableName == table && i.IsPrimary);
            primaryKeys?.Columns.ToList().ForEach(c => result.Add(c.Name, c.Order));

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
