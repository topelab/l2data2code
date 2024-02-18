using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Schema;
using Npgsql;
using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.NpgSql
{
    public class NpgForeignKeysGetter : IForeignKeysGetter<NpgsqlConnection>
    {
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

        public IEnumerable<Key> GetForeignKeys(NpgsqlConnection connection, Tables tables)
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

                var tableReferencing = tables[referencingTable];
                var tableReferenced = tables[referencedTable];

                key.Name = reader["constraint_name"].ToString();
                key.ColumnReferencing = tableReferencing.GetColumn(referencingColumn);
                key.ColumnReferenced = tableReferenced.GetColumn(referencedColumn);

                result.Add(key);
            }

            return result;
        }
    }
}
