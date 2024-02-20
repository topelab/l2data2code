using Dapper;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Schema;
using Npgsql;
using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.NpgSql
{
    public class NpgForeignKeysGetter : IForeignKeysGetter<NpgsqlConnection>
    {
        private record ForeignKeyDTO(string ConstraintName,
                                     string TableName,
                                     string ColumnName,
                                     string ReferencedTableName,
                                     string ReferencedColumnName);

        private const string ALL_FOREIGN_KEYS = """
            select 
            	conname as "ConstraintName",
            	clc.relname as "TableName",
                att2.attname as "ColumnName", 
                cl.relname as "ReferencedTableName", 
                att.attname as "ReferencedColumnName"
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

            var foreignKeys = connection.Query<ForeignKeyDTO>(ALL_FOREIGN_KEYS);

            foreach (var foreignKey in foreignKeys)
            {
                Key key = new();
                var referencingTable = foreignKey.TableName;
                var referencingColumn = foreignKey.ColumnName;
                var referencedTable = foreignKey.ReferencedTableName;
                var referencedColumn = foreignKey.ReferencedColumnName;

                var tableReferencing = tables[referencingTable];
                var tableReferenced = tables[referencedTable];

                key.Name = foreignKey.ConstraintName;
                key.ColumnReferencing = tableReferencing.GetColumn(referencingColumn);
                key.ColumnReferenced = tableReferenced.GetColumn(referencedColumn);

                result.Add(key);
            }

            return result;
        }
    }
}
