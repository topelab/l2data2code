using System;
using System.Collections.Generic;
using System.Text;

namespace L2Data2Code.SharedLib.Configuration
{
    public class TemplateConfiguration
    {
        public string Path { get; set; }
        public string Resource { get; set; } = "General";
        public bool RemoveFolders { get; set; } = true;
    }
}
