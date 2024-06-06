using L2Data2Code.SchemaReader.Schema;
using System.Data;

namespace L2Data2Code.SchemaReader.Interface
{
    public interface IColumnDescriptionsGetter<in T> where T : IDbConnection
    {
        void SetColumnsDescriptions(T connection, Tables tables);
    }
}
