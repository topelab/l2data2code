using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace L2Data2Code.SharedLib.Helpers
{
    public interface IJsonSetting : INotifyPropertyChanged
    {
        JObject Config { get; }

        void AddSettingFiles(params string[] additionalSettingFiles);
        void ReloadSettings(params string[] additionalSettingFiles);
        void Initialize(string settingsFile);
    }
}