using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.Schema
{
    public record IndexColumn(string TableName, string IndexName, string Name, int Order, bool IsDescending);

    public record Index(string TableName, string Name, bool IsUnique, bool IsPrimary, List<IndexColumn> Columns);
}
