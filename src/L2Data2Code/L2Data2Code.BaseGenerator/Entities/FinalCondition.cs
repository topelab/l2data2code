using L2Data2Code.SharedLib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace L2Data2Code.BaseGenerator.Entities
{
    public class FinalCondition
    {
        private JToken thenConfiguration;

        public string When { get; set; }
        public string Eq { get; set; }

        [JsonIgnore]
        public NameValueCollection Then { get; set; }


        [JsonProperty(nameof(Then))]
        public JToken ThenConfiguration
        {
            get => thenConfiguration;
            set
            {
                thenConfiguration = value;
                Then = value.ToNameValueCollection();
            }
        }

        public override string ToString()
        {
            return $"if {When}={Eq} {Then.ToNestedSemiColonSeparatedString()}";
        }
    }
}
