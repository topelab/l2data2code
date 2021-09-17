using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace L2Data2Code.SharedLib.Configuration
{
    public class TemplateConfiguration
    {
        public string Name { get; set; }
        public string Path { get; set; } = string.Empty;
        public bool RemoveFolders { get; set; } = true;
        public string ResourcesFolder { get; set; } = "General";
        public List<string> ItemsResources { get; set; } = new List<string>();
        public string Company { get; set; }
        public string Area { get; set; }
        public string Module { get; set; }
        public string IgnoreColumns { get; set; }
        public string SavePath { get; set; }
        public string SolutionType { get; set; } = "vs,*.sln";
        public string NextResource { get; set; }
        public bool IsGeneral { get; set; }
        public List<Command> PreCommands { get; set; } = new List<Command>();
        public List<Command> PostCommands { get; set; } = new List<Command>();

        [JsonProperty("Configurations")]
        public JToken ConfigurationsConfiguration { get; set; }

        [JsonProperty("Vars")]
        public JToken VarsConfiguration { get; set; }

        [JsonProperty("FinalVars")]
        public JToken FinalVarsConfiguration { get; set; }

        [JsonIgnore]
        public NameValueCollection Configurations { get; set; }

        [JsonIgnore]
        public NameValueCollection Vars { get; set; }

        [JsonIgnore]
        public NameValueCollection FinalVars { get; set; }

    }
}
