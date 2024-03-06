using L2Data2Code.SchemaReader.Schema;
using System.Collections.Generic;
using System.Data;

namespace L2Data2Code.SchemaReader.Interface
{
    public interface IForeignKeysGetter<in T> where T : IDbConnection
    {
        IEnumerable<Key> GetForeignKeys(T connection, Tables tables);
    }
}