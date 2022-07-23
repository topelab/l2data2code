using L2Data2Code.SharedLib.Helpers;

namespace L2Data2Code.SharedLib.Configuration
{
    public class AppSettingsConfiguration : BasicNameValueConfiguration, IAppSettingsConfiguration
    {
        public const string APP_SETTINGS_FILE = "appsettings.json";
        public const string APP_SETTINGS = "appSettings";

        public AppSettingsConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, APP_SETTINGS)
        {
        }
    }
}
