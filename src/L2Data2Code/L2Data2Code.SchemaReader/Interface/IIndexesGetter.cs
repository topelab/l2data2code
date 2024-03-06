using System.Collections.Generic;
using System.Data;
using L2Data2Code.SchemaReader.Schema;

namespace L2Data2Code.SchemaReader.Interface
{
    public interface IIndexesGetter<in T> where T : IDbConnection
    {
        List<Index> GetIndexes(T connection);
    }
}
