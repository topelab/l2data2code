using L2Data2Code.SharedLib.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace L2Data2Code.SharedLib.Configuration
{
    public class BasicConfiguration<T> : IBasicConfiguration<T> where T : class
    {
        private readonly Dictionary<string, T> _values;
        private readonly string list;
        private readonly IJsonSetting jsonSetting;

        public BasicConfiguration(IJsonSetting jsonSetting, string list)
        {
            this.jsonSetting = jsonSetting;
            this.list = list;
            _values = new Dictionary<string, T>();
            SetupValues(jsonSetting.Config[list]);

            jsonSetting.PropertyChanged += JsonSetting_PropertyChanged;
        }

        private void JsonSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(JsonSetting.Config))
            {
                SetupValues(jsonSetting.Config[list]);
            }
        }

        public IEnumerable<string> GetKeys()
        {
            return _values.Keys;
        }

        public IEnumerable<T> GetValues()
        {
            return _values.Values;
        }

        public T FirstOrDefault()
        {
            return _values.Values.FirstOrDefault();
        }

        public int Count => _values?.Count ?? 0;

        public T this[string key] { get => _values.TryGetValue(key, out var value) ? value : null; set => _values[key] = value; }

        private void SetupValues(JToken token)
        {
            if (token != null)
            {
                foreach (var item in token.Cast<JProperty>())
                {
                    _values[item.Name] = item.Value.ToObject<T>();
                }
            }
        }

    }
}
