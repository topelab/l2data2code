using L2Data2Code.SharedLib.Helpers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace L2Data2Code.SharedLib.Configuration
{
    public class BasicNameValueConfiguration
    {
        private readonly string _list;
        private readonly NameValueCollection _valueCollection;

        public BasicNameValueConfiguration(string list)
        {
            _list = list;
            _valueCollection = ConfigHelper.Config[list].ToNameValueCollection(); //GetNameValueCollection(list);
        }

        public IEnumerable<string> GetKeys()
        {
            return _valueCollection.AllKeys.AsEnumerable();
        }

        public string FirstOrDefault()
        {
            return _valueCollection.Count > 0 ? _valueCollection[0] : null;
        }

        public string this[string key] { get => _valueCollection[key]; set => _valueCollection[key] = value; }

    }
}
