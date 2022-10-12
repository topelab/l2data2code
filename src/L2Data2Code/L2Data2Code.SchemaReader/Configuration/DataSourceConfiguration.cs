using L2Data2Code.SharedLib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace L2Data2Code.SchemaReader.Configuration
{
    /// <summary>
    /// Data source configuration
    /// </summary>
    public class DataSourceConfiguration
    {
        private string outputSchema;
        private JToken varsConfiguration;
        private JToken configurationsConfiguration;

        /// <summary>
        /// Area
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// Default module
        /// </summary>
        public string DefaultModule { get; set; }
        /// <summary>
        /// Schema
        /// </summary>
        public string Schema { get; set; }
        /// <summary>
        /// Output schema
        /// </summary>
        public string OutputSchema { get => outputSchema ?? Schema; set => outputSchema = value; }

        /// <summary>
        /// Vars configuration
        /// </summary>
        [JsonProperty(nameof(Vars))]
        public JToken VarsConfiguration { get => varsConfiguration; set { varsConfiguration = value; Vars = value.ToNameValueCollection(); } }

        /// <summary>
        /// Configurations configuration
        /// </summary>
        [JsonProperty(nameof(Configurations))]
        public JToken ConfigurationsConfiguration { get => configurationsConfiguration; set { configurationsConfiguration = value; Configurations = value.ToNameValueCollection(); } }

        /// <summary>
        /// Vars
        /// </summary>
        [JsonIgnore]
        public NameValueCollection Vars { get; set; }

        /// <summary>
        /// Configurations
        /// </summary>
        [JsonIgnore]
        public NameValueCollection Configurations { get; set; }

        public DataSourceConfiguration()
        {
        }

        public DataSourceConfiguration(string schema)
        {
            Schema = schema;
        }
    }
}
