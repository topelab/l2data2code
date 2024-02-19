using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Schema;
using Npgsql;
using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.NpgSql
{
    public class NpgForeignKeysGetter : IForeignKeysGetter<NpgsqlConnection>
    {
        private const string ALL_FOREIGN_KEYS = """
            select 
            	conname as "constraint_name",
            	clc.relname as "table_name",
                att2.attname as "column_name", 
                cl.relname as "referenced_table_name", 
                att.attname as "referenced_column_name"
            from
               (select 
                    unnest(con1.conkey) as "parent", 
                    unnest(con1.confkey) as "child", 
                    con1.confrelid, 
                    con1.conrelid,
                    con1.conname
                from 
                    pg_class cl
                    join pg_namespace ns on cl.relnamespace = ns.oid
                    join pg_constraint con1 on con1.conrelid = cl.oid
                where ns.nspname = 'public'
                    and con1.contype = 'f'
               ) con
               join pg_attribute att on
                   att.attrelid = con.confrelid and att.attnum = con.child
               join pg_class cl on
                   cl.oid = con.confrelid
               join pg_class clc on
                   clc.oid = con.conrelid
               join pg_attribute att2 on
                   att2.attrelid = con.conrelid and att2.attnum = con.parent;
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
