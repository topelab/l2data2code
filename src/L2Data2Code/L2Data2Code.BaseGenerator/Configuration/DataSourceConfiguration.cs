using L2Data2Code.SharedLib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace L2Data2Code.BaseGenerator.Configuration
{
    /// <summary>
    /// Data source configuration
    /// </summary>
    public class DataSourceConfiguration
    {
        private const string DEFAULT_KEY = "localserver";

        private string outputSchema;
        private JToken varsConfiguration;
        private JToken configurationsConfiguration;
        private JToken settingsConfiguration;
        private JToken modulesConfiguration;
        private string schema;

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

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
        public string Schema { get => schema ?? DEFAULT_KEY; set => schema = value; }
        /// <summary>
        /// Output schema
        /// </summary>
        public string OutputSchema { get => outputSchema ?? Schema; set => outputSchema = value; }

        /// <summary>
        /// Modules
        /// </summary>
        public string ModulesGroup { get; set; }

        /// <summary>
        /// Vars configuration
        /// </summary>
        [JsonProperty(nameof(Vars))]
        public JToken VarsConfiguration
        {
            get => varsConfiguration;
            set
            {
                varsConfiguration = value;
                Vars = value.ToNameValueCollection();
            }
        }

        /// <summary>
        /// Configurations configuration
        /// </summary>
        [JsonProperty(nameof(Configurations))]
        public JToken ConfigurationsConfiguration
        {
            get => configurationsConfiguration;
            set
            {
                configurationsConfiguration = value;
                Configurations = value.ToNameValueCollection();
            }
        }

        /// <summary>
        /// Settings configuration
        /// </summary>
        [JsonProperty(nameof(Settings))]
        public JToken SettingsConfiguration
        {
            get => settingsConfiguration;
            set
            {
                settingsConfiguration = value;
                Settings = value.ToNameValueCollection();
            }
        }

        [JsonIgnore]
        public List<ModuleConfiguration> Modules { get; set; }


        [JsonProperty(nameof(Modules))]
        public JToken ModulesConfiguration
        {
            get => modulesConfiguration;
            set
            {
                modulesConfiguration = value;
                Modules = value.ToListOf<ModuleConfiguration>();
            }
        }

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

        /// <summary>
        /// Settings
        /// </summary>
        [JsonIgnore]
        public NameValueCollection Settings { get; set; }


        public DataSourceConfiguration()
        {
        }

        public DataSourceConfiguration(string schema)
        {
            Schema = schema;
        }
    }
}
