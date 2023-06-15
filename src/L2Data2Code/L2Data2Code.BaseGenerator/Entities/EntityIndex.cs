using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Entities
{
    public record EntityIndex(string Name, bool IsUnique, List<EntityIndexColumn> IndexColumns);
}
