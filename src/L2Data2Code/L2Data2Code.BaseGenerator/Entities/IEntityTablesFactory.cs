using L2Data2Code.SchemaReader.Schema;
using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Entities
{
    public interface IEntityTablesFactory
    {
        Dictionary<string, EntityTable> Create(IEnumerable<Table> tables);
    }
}