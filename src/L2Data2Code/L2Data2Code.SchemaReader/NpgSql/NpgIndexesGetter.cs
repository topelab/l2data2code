using Dapper;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Schema;
using Npgsql;
using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.NpgSql
{
    public class NpgIndexesGetter : IIndexesGetter<NpgsqlConnection>
    {
        private record IndexDTO(string TableName, string IndexName, bool Unique, bool Primary);

        private const string ALL_INDEXES = """
            SELECT 
            	tc.relname as "TableName", 
            	c.relname as "IndexName", 
            	i.indisunique as "Unique",
            	i.indisprimary as "Primary"
            FROM pg_index i
            join pg_class c on c.oid = i.indexrelid
            join pg_class tc on tc.oid = i.indrelid  
            join pg_namespace ns on ns.oid = c.relnamespace
            where ns.nspname = 'public'
            order by 1,2
            """;

        private record IndexColumnDTO(string TableName, string IndexName, bool Unique, bool Primary, string ColumnName, int OrdinalPosition, string SortOrder);

        private const string ALL_INDEXES_COLUMNS = """
            SELECT ct.relname AS "TableName", 
                    ci.relname AS "IndexName", 
                    i.indisunique AS "Unique", 
            		i.indisprimary AS "Primary",
                    pg_get_indexdef(ci.oid, (i.keys).n, false) AS "ColumnName", 
                    (i.keys).n AS "OrdinalPosition", 
                    CASE pg_indexam_has_property(am.oid, 'can_order') 
                        WHEN true THEN CASE i.indoption[(i.keys).n - 1] & 1 
                        WHEN 1 THEN 'D' 
                        ELSE 'A' 
                        END 
                        ELSE NULL 
                    END AS "SortOrder"
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


        public List<Index> GetIndexes(NpgsqlConnection connection)
        {
            var indexes = GetIndexHeaders(connection);
            return GetIndexColumns(connection, indexes);
        }

        private Dictionary<string, Index> GetIndexHeaders(NpgsqlConnection connection)
        {
            Dictionary<string, Index> indexes = [];

            var allIndexes = connection.Query<IndexDTO>(ALL_INDEXES);

            foreach (var indexDTO in allIndexes)
            {
                var tableName = indexDTO.TableName;
                var indexName = indexDTO.IndexName;
                var index = new Index(tableName, indexName, indexDTO.Unique, indexDTO.Primary, new List<IndexColumn>());
                indexes.Add(indexName, index);
            }

            return indexes;
        }

        private List<Index> GetIndexColumns(NpgsqlConnection connection, Dictionary<string, Index> indexes)
        {
            var allIndexColumns = connection.Query<IndexColumnDTO>(ALL_INDEXES_COLUMNS);

            foreach (var indexColumnDTO in allIndexColumns)
            {
                var tableName = indexColumnDTO.TableName;
                var indexName = indexColumnDTO.IndexName;
                var indexColumn = new IndexColumn(tableName, indexName, indexColumnDTO.ColumnName, indexColumnDTO.OrdinalPosition, indexColumnDTO.SortOrder != "A");
                if (indexes.TryGetValue(indexName, out var index))
                {
                    index.Columns.Add(indexColumn);
                }
            }

            return [.. indexes.Values];
        }
    }
}
