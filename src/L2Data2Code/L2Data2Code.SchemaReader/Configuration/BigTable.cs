using L2Data2Code.SharedLib.Interfaces;
using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.Configuration
{
    public class BigTable : IKeyed
    {
        public string Key { get; set; }
        public List<string> ColumnsFilter { get; set; } = [];
    }
}