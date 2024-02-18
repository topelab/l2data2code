using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Schema;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace L2Data2Code.SchemaReader.NpgSql
{
    public class NpgIndexesGetter : IIndexesGetter<NpgsqlConnection>
    {
        private const string ALL_INDEXES = """
            SELECT 
            	tc.relname as table_name, 
            	c.relname as index_name, 
            	i.indisunique as unique,
            	i.indisprimary as primary
            FROM pg_index i
            join pg_class c on c.oid = i.indexrelid
            join pg_class tc on tc.oid = i.indrelid  
            join pg_namespace ns on ns.oid = c.relnamespace
            where ns.nspname = 'public'
            order by 1,2
            """;

        private enum IndexFields
        {
            TableName,
            IndexName,
            Unique,
            Primary
        }

        private const string ALL_INDEXES_COLUMNS = """
            SELECT ct.relname AS table_name, 
                    i.indisunique AS unique, 
            		i.indisprimary AS primary,
                    ci.relname AS index_name, 
                    (i.keys).n AS ordinal_position, 
                    pg_get_indexdef(ci.oid, (i.keys).n, false) AS column_name, 
                    CASE pg_indexam_has_property(am.oid, 'can_order') 
                        WHEN true THEN CASE i.indoption[(i.keys).n - 1] & 1 
                        WHEN 1 THEN 'D' 
                        ELSE 'A' 
                        END 
                        ELSE NULL 
                    END AS sort_order,
                    pg_get_expr(i.indpred, i.indrelid) AS filter_condition 
            FROM pg_class ct 
                JOIN pg_namespace n ON (ct.relnamespace = n.oid) 
                JOIN (SELECT i.indexrelid, i.indrelid, i.indoption, 
                        i.indisunique, i.indisclustered, i.indpred, 
                        i.indexprs, i.indisprimary,
                        information_schema._pg_expandarray(i.indkey) AS keys 
                    FROM pg_index i) i 
                ON (ct.oid = i.indrelid) 
                JOIN pg_class ci ON (ci.oid = i.indexrelid) 
                JOIN pg_am am ON (ci.relam = am.oid) 
            WHERE n.nspname = 'public';
            """;

        private enum IndexColumnFields
        {
            TableName,
            Unique,
            Primary,
            IndexName,
            OrdinalPosition,
            ColumnName,
            SortOrder,
            FilterCondition
        }

        public List<Index> GetIndexes(NpgsqlConnection connection)
        {
            Dictionary<string, Index> indexes = GetIndexHeaders(connection);
            return GetIndexColumns(connection, indexes);
        }

        private Dictionary<string, Index> GetIndexHeaders(NpgsqlConnection connection)
        {
            Dictionary<string, Index> indexes = [];
            using var cmd = connection.CreateCommand();
            cmd.CommandText = ALL_INDEXES;

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string tableName = reader.GetString((int)IndexFields.TableName);
                string indexName = reader.GetString((int)IndexFields.IndexName);
                var index = new Index(tableName, indexName, reader.GetBoolean((int)IndexFields.Unique), reader.GetBoolean((int)IndexFields.Primary), new List<IndexColumn>());
                indexes.Add(indexName, index);
            }

            return indexes;
        }

        private List<Index> GetIndexColumns(NpgsqlConnection connection, Dictionary<string, Index> indexes)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = ALL_INDEXES_COLUMNS;

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string tableName = reader.GetString((int)IndexColumnFields.TableName);
                string indexName = reader.GetString((int)IndexColumnFields.IndexName);
                var indexColumn = new IndexColumn(tableName, indexName, reader.GetString((int)IndexColumnFields.ColumnName), reader.GetInt32((int)IndexColumnFields.OrdinalPosition), reader.GetString((int)IndexColumnFields.SortOrder) != "A");
                if (indexes.TryGetValue(indexName, out var index))
                {
                    index.Columns.Add(indexColumn);
                }
            }

            return indexes.Values.ToList();
        }
    }
}
