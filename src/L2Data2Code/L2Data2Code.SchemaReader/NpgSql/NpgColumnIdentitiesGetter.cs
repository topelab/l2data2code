using Dapper;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Schema;
using Npgsql;

namespace L2Data2Code.SchemaReader.NpgSql
{
    internal class NpgColumnIdentitiesGetter : IColumnIdentitiesGetter<NpgsqlConnection>
    {
        private record IdentityColumnDTO(string TableName, string ColumnName, char Identity);

        private const string ALL_IDENTITY_COLUMNS = """
            select c.relname as "TableName", a.attname as "ColumnName", a.attidentity as "Identity"
             from pg_attribute as a
             inner join pg_class as c on a.attrelid = c.oid
            where a.attnum > 0 and a.attidentity <> '';
            """;

        public void SetColumnsIdentities(NpgsqlConnection connection, Tables tables)
        {
            var identities = connection.Query<IdentityColumnDTO>(ALL_IDENTITY_COLUMNS);
            foreach (var identity in identities)
            {
                if (tables.TryGetValue(identity.TableName, out var table)
                    && table.TryGetColumn(identity.ColumnName) is Column column)
                {
                    column.IsAutoIncrement = true;
                }
            }
        }
    }
}
