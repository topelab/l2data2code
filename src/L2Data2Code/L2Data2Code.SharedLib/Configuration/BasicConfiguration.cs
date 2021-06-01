using L2Data2Code.SharedLib.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace L2Data2Code.SharedLib.Configuration
{
    public class BasicConfiguration<T> where T : class
    {
        private readonly Dictionary<string, T> _values;

        public BasicConfiguration(JToken token)
        {
            _values = new Dictionary<string, T>();
            foreach (JProperty item in token)
            {
                _values.Add(item.Name, item.Value.ToObject<T>());
            }
        }

        public BasicConfiguration(string list) : this(ConfigHelper.Config[list])
        {
        }

        public IEnumerable<string> GetKeys()
        {
            return _values.Keys;
        }

        public T FirstOrDefault()
        {
            return _values.Values.FirstOrDefault();
        }

        public int Count => _values?.Count ?? 0;

        public T this[string key] { get => _values.ContainsKey(key) ? _values[key] : null; set => _values[key] = value; }

    }
}
