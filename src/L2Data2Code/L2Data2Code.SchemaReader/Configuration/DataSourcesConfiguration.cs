using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Helpers;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public class DataSourcesConfiguration : BasicConfiguration<DataSourceConfiguration>, IDataSorcesConfiguration
    {
        private const string DEFAULT_KEY = "localserver";
        public DataSourcesConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, ConfigurationLabels.DATA_SOURCES)
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
            foreach (var key in GetKeys())
            {
                if (this[key].ModulesGroup == null)
                {
                    this[key].ModulesGroup = this[key].DefaultModule;
                }
            }
        }

        public string Schema(string key)
        {
            var value = this[key];
            return value?.Schema ?? DEFAULT_KEY;
        }

        public string OutputSchema(string key)
        {
            var value = this[key];
            var defaultKey = Schema(key);
            return value?.OutputSchema ?? defaultKey;
        }
    }
}
