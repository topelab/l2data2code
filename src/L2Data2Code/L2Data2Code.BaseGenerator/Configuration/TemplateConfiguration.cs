using L2Data2Code.BaseGenerator.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public class TemplateConfiguration
    {
        public string Name { get; set; }
        public string Path { get; set; } = string.Empty;
        public bool RemoveFolders { get; set; } = true;
        public string ResourcesFolder { get; set; } = Constants.GeneralResourceFolder;
        public List<string> ItemsResources { get; set; } = [];
        public string Partials { get; set; }
        public string Company { get; set; }
        public string Area { get; set; }
        public string Module { get; set; }
        public string IgnoreColumns { get; set; }
        public string SavePath { get; set; }
        public string SolutionType { get; set; } = "vs,*.sln";
        public string NextResource { get; set; }
        public bool IsGeneral { get; set; }

        [JsonIgnore]
        public List<Command> PreCommands { get; set; } = [];

        [JsonIgnore]
        public List<Command> PostCommands { get; set; } = [];

        [JsonIgnore]
        public NameValueCollection Vars { get; set; }

        [JsonIgnore]
        public List<Setting> Settings { get; set; } = [];

        [JsonIgnore]
        public List<FinalCondition> FinalConditions { get; set; }


        [JsonProperty(nameof(PostCommands))]
        public JToken PostCommandsConfiguration { get; set; }

        [JsonProperty(nameof(PreCommands))]
        public JToken PreCommandsConfiguration { get; set; }

        [JsonProperty(nameof(Vars))]
        public JToken VarsConfiguration { get; set; }

        [JsonProperty(nameof(Settings))]
        public JToken SettingsConfiguration { get; set; }

        [JsonProperty(nameof(FinalConditions))]
        public JToken FinalConditionsConfiguration { get; set; }

    }
}
