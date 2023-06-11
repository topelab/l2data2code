using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace L2Data2Code.SchemaReader.SqlServer
{
    public class SqlServerSchemaReader : Schema.SchemaReader
    {
        private readonly INameResolver nameResolver;
        private Dictionary<string, string> columnsDescriptions = new();
        private readonly string connectionString;
        private IDbConnection connection;


        public SqlServerSchemaReader(INameResolver nameResolver, ISchemaOptions options) : base(options.SummaryWriter)
        {
            connectionString = options.ConnectionString;
            this.nameResolver = nameResolver ?? throw new ArgumentNullException(nameof(nameResolver));
            nameResolver.Initialize(options.SchemaName);
        }

        public override bool CanConnect()
        {
            try
            {
                using SqlConnection connection = new(connectionString);
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override Tables ReadSchema(SchemaReaderOptions options)
        {
            columnsDescriptions = GetTableDescriptions(connectionString);
            Tables result = new();

            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //pull the tables in a reader
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = TABLES_SQL;

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Table tbl = new()
                            {
                                Name = (string)rdr["TABLE_NAME"],
                                SourceDB = "sqlserver"
                            };

                            if (options.TableRegex != null && !options.TableRegex.IsMatch(tbl.Name))
                            {
                                continue;
                            }

                            tbl.Schema = (string)rdr["TABLE_SCHEMA"];
                            tbl.IsView = string.Compare((string)rdr["TABLE_TYPE"], "VIEW", true) == 0;
                            tbl.CleanName = RemoveTablePrefixes(nameResolver.ResolveTableName(tbl.Name)).PascalCamelCase(false);
                            tbl.Type = nameResolver.ResolveTableType(tbl.Name);
                            tbl.ClassName = tbl.CleanName.ToSingular();
                            tbl.Description = columnsDescriptions.TryGetValue(tbl.Name, out var value) ? value
                                : options.AlternativeDescriptions.TryGetValue(tbl.Name, out var alternativeValue) ? alternativeValue : null;

                            result.Add(tbl.Name, tbl);
                        }
                    }
                    cmd.CommandText = VIEWS_SQL;

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Table tbl = new()
                            {
                                Name = (string)rdr["TABLE_NAME"],
                                SourceDB = "sqlserver"
                            };

                            if (options.TableRegex != null && !options.TableRegex.IsMatch(tbl.Name))
                            {
                                continue;
                            }

                            tbl.Schema = (string)rdr["TABLE_SCHEMA"];
                            tbl.IsView = true;
                            tbl.IsUpdatable = (string)rdr["IS_UPDATABLE"] == "YES";
                            tbl.CleanName = RemoveTablePrefixes(nameResolver.ResolveTableName(tbl.Name)).PascalCamelCase(false);
                            tbl.Type = nameResolver.ResolveTableType(tbl.Name);
                            tbl.ClassName = tbl.CleanName.ToSingular();
                            tbl.Description = columnsDescriptions.TryGetValue(tbl.Name, out var value) ? value : null;

                            result.Add(tbl.Name, tbl);
                        }
                    }
                }


                try
                {
                    foreach (var tbl in result.Values)
                    {
                        tbl.Columns = LoadColumns(tbl, options.RemoveFirstWord);

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

        private List<Column> LoadColumns(Table tbl, bool removeFirstWord = true)
        {

            using var cmd = connection.CreateCommand();
            cmd.CommandText = COLUMNS_SQL;

            var p = cmd.CreateParameter();
            p.ParameterName = "@tableName";
            p.Value = tbl.Name;
            cmd.Parameters.Add(p);

            p = cmd.CreateParameter();
            p.ParameterName = "@schemaName";
            p.Value = tbl.Schema;
            cmd.Parameters.Add(p);

            List<Column> result = new();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Column col = new()
                    {
                        Table = tbl,
                        TableName = tbl.Name,
                        Owner = (string)rdr["Owner"],
                        Name = (string)rdr["ColumnName"]
                    };
                    col.PropertyName = nameResolver.ResolveColumnName(tbl.Name, col.Name).PascalCamelCase(removeFirstWord);
                    col.PropertyType = GetPropertyType((string)rdr["DataType"]);
                    col.IsNullable = (string)rdr["IsNullable"] == "YES";
                    col.IsAutoIncrement = ((int)rdr["IsIdentity"]) == 1;
                    col.Description = columnsDescriptions.TryGetValue(col.FullName, out var value) ? value : null;
                    col.Precision = (int)rdr["Precision"];
                    col.NumericScale = (int)rdr["NumericScale"];
                    col.IsNumeric = rdr["NUMERIC_PRECISION"].IfNull(0) > 0;
                    col.IsComputed = tbl.IsView || (int)rdr["IsComputed"] != 0 || ((string)rdr["DataType"]).Equals("timestamp");
                    col.DefaultValue = rdr["DefaultSetting"].IfNull<string>(null) == null ? null : ((string)rdr["DefaultSetting"]).RemoveOuter('(', ')').RemoveOuter('\'').Replace("getdate()", "DateTime.Now", StringComparison.CurrentCultureIgnoreCase);
                    if (col.DefaultValue != null && col.PropertyType == "decimal" && !col.DefaultValue.EndsWith("m") && col.DefaultValue.Contains('.'))
                    {
                        col.DefaultValue += "m";
                    }

                    result.Add(col);
                }
            }

            return result;
        }

        private List<Key> LoadRelations(Tables tables)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = ALL_FOREIGN_KEYS;

            List<Key> result = new();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Key key = new();
                    var referencingTable = (string)rdr["ReferencingTable"];
                    var referencingColumn = (string)rdr["ReferencingColumn"];
                    var referencedTable = (string)rdr["ReferencedTable"];
                    var referencedColumn = (string)rdr["ReferencedColumn"];

                    WriteLine($"// referencedColumn: {referencedColumn}, referencingColumn: {referencingColumn}");

                    var tableReferencing = tables[referencingTable];
                    var tableReferenced = tables[referencedTable];

                    key.Name = rdr["Name"].ToString();
                    key.ColumnReferencing = tableReferencing.GetColumn(referencingColumn);
                    key.ColumnReferenced = tableReferenced.GetColumn(referencedColumn);

                    result.Add(key);
                }
            }

            return result;
        }

        private Dictionary<string, int> GetPK(string table)
        {

            Dictionary<string, int> result = new();

            var sql = @"SELECT c.name AS ColumnName, ic.index_column_id as ""Order""
                FROM sys.indexes AS i 
                INNER JOIN sys.index_columns AS ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id 
                INNER JOIN sys.objects AS o ON i.object_id = o.object_id 
                LEFT OUTER JOIN sys.columns AS c ON ic.object_id = c.object_id AND c.column_id = ic.column_id
                WHERE (i.is_primary_key = 1) AND (o.name = @tableName)";

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;

                var p = cmd.CreateParameter();
                p.ParameterName = "@tableName";
                p.Value = table;
                cmd.Parameters.Add(p);

                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.Add(rdr.GetString(0), rdr.GetInt32(1));
                }

            }

            return result;
        }

        private List<Schema.Index> GetIndexes(string table)
        {

            List<Schema.Index> result = new();

            var sql = @"SELECT i.name as IndexName, c.name AS ColumnName, ic.index_column_id as ""Order"", i.is_unique
                FROM sys.indexes AS i 
                INNER JOIN sys.index_columns AS ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id 
                INNER JOIN sys.objects AS o ON i.object_id = o.object_id 
                LEFT OUTER JOIN sys.columns AS c ON ic.object_id = c.object_id AND c.column_id = ic.column_id
                WHERE (i.is_primary_key = 0) AND (i.is_unique_constraint = 0) AND (o.name = @tableName)
                ORDER BY 1,3";

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;

                var p = cmd.CreateParameter();
                p.ParameterName = "@tableName";
                p.Value = table;
                cmd.Parameters.Add(p);

                using var rdr = cmd.ExecuteReader();
                var lastIndexName = string.Empty;
                Dictionary<string, int> fields = new();
                var isUnique = false;

                while (rdr.Read())
                {
                    var indexName = rdr.GetString(0);
                    var fieldName = rdr.GetString(1);
                    var fieldOrder = rdr.GetInt32(2);
                    isUnique = rdr.GetBoolean(3);

                    if (indexName != lastIndexName)
                    {
                        if (!string.IsNullOrEmpty(lastIndexName))
                        {
                            AddIndex(result, lastIndexName, fields, isUnique);
                        }
                        fields.Clear();
                    }
                    fields.Add(fieldName, fieldOrder);
                }

                if (fields.Any() && !string.IsNullOrEmpty(lastIndexName))
                {
                    AddIndex(result, lastIndexName, fields, isUnique);
                }
            }

            return result;
        }

        private static void AddIndex(List<Schema.Index> result, string lastIndexName, Dictionary<string, int> fields, bool isUnique)
        {
            var index = new Schema.Index { Name = lastIndexName, IsUnique = isUnique };
            index.Columns.AddRange(fields);
            result.Add(index);
        }

        private static string GetPropertyType(string sqlType)
        {
            var sysType = "string";
            switch (sqlType)
            {
                case "bigint":
                    sysType = "long";
                    break;
                case "smallint":
                    sysType = "short";
                    break;
                case "int":
                    sysType = "int";
                    break;
                case "uniqueidentifier":
                    sysType = "Guid";
                    break;
                case "smalldatetime":
                case "datetime":
                case "datetime2":
                case "date":
                case "timestamp":
                    sysType = "DateTime";
                    break;
                case "time":
                    sysType = "TimeSpan";
                    break;
                case "float":
                    sysType = "double";
                    break;
                case "real":
                    sysType = "float";
                    break;
                case "numeric":
                case "smallmoney":
                case "decimal":
                case "money":
                    sysType = "decimal";
                    break;
                case "tinyint":
                    sysType = "byte";
                    break;
                case "bit":
                    sysType = "bool";
                    break;
                case "image":
                case "binary":
                case "varbinary":
                    sysType = "byte[]";
                    break;
                case "geography":
                    sysType = "Microsoft.SqlServer.Types.SqlGeography";
                    break;
                case "geometry":
                    sysType = "Microsoft.SqlServer.Types.SqlGeometry";
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


        private Dictionary<string, string> GetTableDescriptions(string connectionString)
        {
            Dictionary<string, string> result = new();

            try
            {
                using SqlConnection conn = new(connectionString);
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select 
                        st.name + '.' + sc.name as [Column], sep.value as [Description]
                        from sys.tables st
                        inner join sys.columns sc on st.object_id = sc.object_id
                        left join sys.extended_properties sep on st.object_id = sep.major_id
                                        and sc.column_id = sep.minor_id
                                        and sep.name = 'MS_Description'
                        union
                        SELECT sys.objects.name AS [Column],
                               ep.value AS [Description]
                        FROM sys.objects
                        CROSS APPLY fn_listextendedproperty(default,
                                                            'SCHEMA', schema_name(schema_id),
                                                            'TABLE', name, null, null) ep
                        WHERE sys.objects.name NOT IN ('sysdiagrams')
                            and ep.name = 'MS_Description'
                        ORDER by [Column]";

                    using IDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if (!rdr.IsDBNull(1))
                        {
                            result.Add(rdr.GetString(0), rdr.GetString(1));
                        }
                    }

                }

                conn.Close();
            }
            catch (Exception ex)
            {
                HasErrorMessage(true);
                var error = ex.Message.ReplaceEndOfLine();
                WriteLine("");
                WriteLine("// -----------------------------------------------------------------------------------------");
                WriteLine($"// Failed to Connect to comment server - {error}");
                WriteLine("// -----------------------------------------------------------------------------------------");
                WriteLine("");
                return result;
            }

            return result;
        }

        private const string TABLES_SQL = @"SELECT *
            FROM  INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE='BASE TABLE'
            ORDER BY TABLE_TYPE, TABLE_NAME";

        private const string VIEWS_SQL = @"SELECT *
            FROM  INFORMATION_SCHEMA.VIEWS
            ORDER BY TABLE_NAME";

        private const string COLUMNS_SQL = @"SELECT 
            TABLE_CATALOG AS [Database],
            TABLE_SCHEMA AS Owner, 
            TABLE_NAME AS TableName, 
            COLUMN_NAME AS ColumnName, 
            ORDINAL_POSITION AS OrdinalPosition, 
            COLUMN_DEFAULT AS DefaultSetting, 
            IS_NULLABLE AS IsNullable, DATA_TYPE AS DataType, 
            COALESCE(CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, DATETIME_PRECISION, 0) AS [Precision],
            COALESCE(NUMERIC_SCALE, 0) AS [NumericScale],
            COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsIdentity') AS IsIdentity,
            COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsComputed') as IsComputed,
            NUMERIC_PRECISION
            FROM  INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME=@tableName AND TABLE_SCHEMA=@schemaName
            ORDER BY OrdinalPosition ASC";

        private const string ALL_FOREIGN_KEYS = @"SELECT 
            [Name] = OBJECT_NAME(pt.constraint_object_id),
            [ReferencingSchema] = OBJECT_SCHEMA_NAME(pt.parent_object_id),
            ReferencingTable = OBJECT_NAME(pt.parent_object_id),
            ReferencingColumn = pc.name, 
            [ReferencedSchema] = OBJECT_SCHEMA_NAME(pt.referenced_object_id),
            ReferencedTable = OBJECT_NAME(pt.referenced_object_id),
            ReferencedColumn = rc.name
            FROM sys.foreign_key_columns AS pt
            INNER JOIN sys.columns AS pc
            ON pt.parent_object_id = pc.[object_id]
            AND pt.parent_column_id = pc.column_id
            INNER JOIN sys.columns AS rc
            ON pt.referenced_column_id = rc.column_id
            AND pt.referenced_object_id = rc.[object_id]";

    }

}
