using Dapper;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Schema;
using Npgsql;
using System.Linq;

namespace L2Data2Code.SchemaReader.NpgSql
{
    public class NpgColumnDescriptionsGetter : IColumnDescriptionsGetter<NpgsqlConnection>
    {
        private record DescriptionsDTO(string TableName,
                                     string ColumnName,
                                     string Description);

        private const string ALL_COLUMNS_DESCRIPTIONS = """
            SELECT c.table_name as "TableName", c.column_name as "ColumnName",
                COL_DESCRIPTION(CONCAT(c.table_schema, '."', 
                c.table_name,'"')::regclass, ordinal_position) as "Description"
            FROM information_schema.columns as c
            JOIN information_schema.tables as t
                ON t.table_catalog = c.table_catalog
                AND t.table_schema = c.table_schema
                AND t.table_name = c.table_name
            WHERE c.table_schema = 'public'
            ORDER BY c.table_name, c.ordinal_position;
            """;

        public void SetColumnsDescriptions(NpgsqlConnection connection, Tables tables)
        {
            var descriptions = connection.Query<DescriptionsDTO>(ALL_COLUMNS_DESCRIPTIONS);

            foreach (var description in descriptions.Where(d => !string.IsNullOrEmpty(d.Description)))
            {
                if (tables.TryGetValue(description.TableName, out var table)
                    && table.TryGetColumn(description.ColumnName) is Column column
                    && string.IsNullOrEmpty(column.Description))
                {
                    column.Description = description.Description;
                }
            }
        }
    }
}
