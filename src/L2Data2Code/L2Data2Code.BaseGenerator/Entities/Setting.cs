using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace L2Data2Code.BaseGenerator.Entities
{
    public class Setting: IKeyed
    {
        private JToken varsConfiguration;

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Remove folder exceptions
        /// </summary>
        public List<string> RemoveFolderExceptions { get; set; } = [];

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
        /// Vars
        /// </summary>
        [JsonIgnore]
        public NameValueCollection Vars { get; set; }
    }
}
