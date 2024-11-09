using L2Data2Code.SchemaReader.Schema;
using System.Data;

namespace L2Data2Code.SchemaReader.Interface
{
    public interface IColumnIdentitiesGetter<in T> where T : IDbConnection
    {
        void SetColumnsIdentities(T connection, Tables tables);
    }
}
