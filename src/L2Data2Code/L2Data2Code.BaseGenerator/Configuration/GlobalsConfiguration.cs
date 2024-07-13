using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public class GlobalsConfiguration : BasicConfiguration<JToken>, IGlobalsConfiguration
    {


        public NameValueCollection Vars { get; private set; }
        public NameValueCollection FinalVars { get; private set; }
        [JsonIgnore]
        public List<FinalCondition> FinalConditions { get; set; }

        [JsonProperty(nameof(FinalConditions))]
        public JToken FinalConditionsConfiguration { get; set; }

        public GlobalsConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, GeneratorSectionLabels.GLOBAL)
        {
            SetVars();
            jsonSetting.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(JsonSetting.Config))
                {
                    SetVars();
                }
            };
        }

        private void SetVars()
        {
            Vars = this[nameof(Vars)].ToNameValueCollection();
            FinalVars = this[nameof(FinalVars)].ToNameValueCollection();
            FinalConditions = this[nameof(FinalConditions)].ToListOf<FinalCondition>();
        }
    }
}
