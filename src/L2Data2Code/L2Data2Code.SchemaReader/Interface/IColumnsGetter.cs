using L2Data2Code.SchemaReader.Schema;
using System.Collections.Generic;
using System.Data;

namespace L2Data2Code.SchemaReader.Interface
{
    public interface IColumnsGetter<in T> where T : IDbConnection
    {
        List<Column> GetColumns(T connection, Table table, SchemaReaderOptions options, INameResolver nameResolver);
    }
}
