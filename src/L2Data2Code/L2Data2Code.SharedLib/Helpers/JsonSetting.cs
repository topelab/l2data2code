using L2Data2Code.SharedLib.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace L2Data2Code.SharedLib.Helpers
{
    public class JsonSetting : IJsonSetting
    {
        private JObject config;
        private string settingsFile;

        public JObject Config => config;
        public event PropertyChangedEventHandler PropertyChanged;

        public JsonSetting(string settingsFile)
        {
            SettingsFile(settingsFile);
        }

        public JsonSetting()
        {
            SettingsFile(FileLabels.APP_SETTINGS_FILE);
        }

        public void SettingsFile(string settingsFile)
        {
            this.settingsFile = settingsFile;
            config = GetSettings(settingsFile);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Config)));
        }

        public void ReloadSettings(params string[] additionalSettingFiles)
        {
            config = GetSettings(this.settingsFile);
            AddSettingFiles(additionalSettingFiles);
        }

        public void AddSettingFiles(params string[] additionalSettingFiles)
        {
            foreach (var additionalSettingFile in additionalSettingFiles)
            {
                var addedConfig = GetSettings(additionalSettingFile);
                config.Merge(addedConfig);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Config)));
        }

        private static JObject GetSettings(string settingsFile)
        {
            JObject config;

            if (File.Exists(settingsFile))
            {
                var text = File.ReadAllText(settingsFile);
                config = (JObject)JsonConvert.DeserializeObject(text);
            }
            else
            {
                config = new JObject();
            }

            return config;
        }
    }
}
