using System;
using System.Collections.Generic;
using System.Text;

namespace L2Data2Code.SharedLib.Configuration
{
    public class ModuleConfiguration
    {
        public string Name { get; set; }
        public string IncludeTables { get; set; }
        public string ExcludeTables { get; set; }
    }
}
