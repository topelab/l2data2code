using L2Data2Code.SharedLib.Helpers;

namespace L2Data2Code.SharedLib.Configuration
{
    public class AppSettingsConfiguration : BasicNameValueConfiguration, IAppSettingsConfiguration
    {
        public AppSettingsConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, APP_SETTINGS)
        {
        }
    }
}
