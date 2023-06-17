using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.Schema
{
    public record IndexColumn(string Name, int Order, bool IsDescending);

    public record Index(string Name, bool IsUnique, List<IndexColumn> Columns);
}
