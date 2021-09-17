using System.Collections.Generic;
using System.Collections.Specialized;

namespace L2Data2Code.SharedLib.Configuration
{
    public interface IBasicNameValueConfiguration
    {
        string this[string key] { get; set; }

        string FirstOrDefault();
        IEnumerable<string> GetKeys();
        void Merge(NameValueCollection nameValueCollection);
    }
}