using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public class GlobalConfiguration
    {
        [JsonProperty("Vars")]
        public JToken VarsConfiguration { get; set; }

        [JsonProperty("FinalVars")]
        public JToken FinalVarsConfiguation { get; set; }

        [JsonIgnore]
        public NameValueCollection Vars { get; set; }

        [JsonIgnore]
        public NameValueCollection FinalVars { get; set; }

    }
}