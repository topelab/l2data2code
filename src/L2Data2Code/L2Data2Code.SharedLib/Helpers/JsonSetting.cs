using L2Data2Code.SharedLib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace L2Data2Code.SharedLib.Helpers
{
    public class JsonSetting
    {
        //public IConfiguration Config { get; }
        public JObject Config { get;  }

        public JsonSetting(string settingsFile = "appsettings.json") //, string additionalSettingsFile = null, bool loadEnvirontment = false)
        {
            if (File.Exists(settingsFile))
            {
                var text = File.ReadAllText(settingsFile);
                Config = (JObject)JsonConvert.DeserializeObject(text);
            }
            else
            {
                Config = new JObject();
            }
        }

    }
}
