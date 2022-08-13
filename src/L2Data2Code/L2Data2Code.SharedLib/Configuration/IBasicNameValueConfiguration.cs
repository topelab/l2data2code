using System.Collections.Generic;

namespace L2Data2Code.SharedLib.Configuration
{
    public interface IBasicNameValueConfiguration
    {
        string this[string key] { get; set; }

        string FirstOrDefault();
        IEnumerable<string> GetKeys();
        void Merge(params string[] additionalSettingFiles);
    }
}