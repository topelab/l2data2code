using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Specialized;
using L2Data2Code.SharedLib.Extensions;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public class DataSourceConfiguration
    {
        private string _outputSchema;
        private string _descriptionSchema;
        private JToken varsConfiguration;

        public string Area { get; set; }
        public string Schema { get; set; }
        public string OutputSchema { get => _outputSchema ?? Schema; set => _outputSchema = value; }
        public string DescriptionsSchema { get => _descriptionSchema ?? Schema; set => _descriptionSchema = value; }

        [JsonProperty("Vars")]
        public JToken VarsConfiguration { get => varsConfiguration; set { varsConfiguration = value; Vars = value.ToNameValueCollection(); } }

        [JsonIgnore]
        public NameValueCollection Vars { get; set; }

        public DataSourceConfiguration()
        {
        }

        public DataSourceConfiguration(string schema)
        {
            Schema = schema;
        }
    }
}
