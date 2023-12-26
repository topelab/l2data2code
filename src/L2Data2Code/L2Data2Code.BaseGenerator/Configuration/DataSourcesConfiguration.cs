using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Helpers;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public class DataSourcesConfiguration : BasicConfiguration<DataSourceConfiguration>, IDataSorcesConfiguration
    {
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
                this[key].Key = key;
                if (this[key].ModulesGroup == null)
                {
                    this[key].ModulesGroup = this[key].DefaultModule;
                }
            }
        }
    }
}
