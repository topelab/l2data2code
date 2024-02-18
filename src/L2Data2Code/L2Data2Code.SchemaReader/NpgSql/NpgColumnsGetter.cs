using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace L2Data2Code.SchemaReader.NpgSql
{
    public class NpgColumnsGetter : IColumnsGetter<NpgsqlConnection>
    {
        public List<Column> GetColumns(NpgsqlConnection connection, Table table, SchemaReaderOptions options, INameResolver nameResolver)
        {
            List<Column> result = [];

            string[] schema = [connection.Database, NpgSchemaReader.DefaultDBSchema, table.Name, null];

            // Columnas
            var SchemaTabla = connection.GetSchema("Columns", schema);
            foreach (DataRow row in SchemaTabla.Rows)
            {
                Column col = new()
                {
                    Table = table,
                    TableName = table.Name,
                    Owner = null,
                    Name = (string)row["column_name"],
                    Precision = (int)row["character_maximum_length"].IfNull(row["numeric_precision"].IfNull(row["datetime_precision"].IfNull((ulong)0)))
                };
                col.PropertyName = nameResolver.ResolveColumnName(table.Name, col.Name).PascalCamelCase(options.RemoveFirstWord);
                col.PropertyType = GetPropertyType((string)row["data_type"], col.Precision);
                col.Precision = col.PropertyType == "bool" ? 1 : col.Precision;
                col.IsNullable = ((string)row["is_nullable"]) == "YES";
                col.NumericScale = (int)row["numeric_scale"].IfNull((ulong)0);
                col.IsNumeric = row["numeric_precision"].IfNull(0) > 0;
                col.IsComputed = table.IsView;
                col.DefaultValue = row["column_default"].IfNull<string>(null) == null ? null : ((string)row["column_default"]).RemoveOuter('(', ')').RemoveOuter('\'').Replace("current_timestamp", "DateTime.Now", StringComparison.CurrentCultureIgnoreCase);
                col.Description = options.AlternativeDescriptions != null && options.AlternativeDescriptions.TryGetValue(table.Name, out var value) ? value : string.Empty;
                if (col.DefaultValue != null && col.PropertyType == "decimal" && !col.DefaultValue.EndsWith("m") && col.DefaultValue.Contains('.'))
                {
                    col.DefaultValue += "m";
                }
                result.Add(col);
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


    }
}
