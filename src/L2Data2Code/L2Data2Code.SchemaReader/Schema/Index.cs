using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.Schema
{
    public class Index
    {
        public string Name { get; set; }
        public Dictionary<string, int> Columns { get; set; } = new Dictionary<string, int>();
        public bool IsUnique { get; set; }
    }
}
