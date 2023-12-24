using L2Data2Code.SharedLib.Interfaces;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace L2Data2Code.SharedLib.Extensions
{
    public static class JSonExtensions
    {
        public static NameValueCollection ToNameValueCollection(this JToken source)
        {
            NameValueCollection valueCollection = new();
            if (source != null)
            {
                foreach (var item in source.Cast<JProperty>())
                {
                    valueCollection[item.Name] = (string)item.Value;
                }
            }
            return valueCollection;
        }

        public static bool HasValues(this JArray array)
        {
            return array.HasValues;
        }

        public static List<T> ToListOf<T>(this JToken source) where T : class
        {
            List<T> list = [];
            if (source != null)
            {
                foreach (var item in source.Cast<JProperty>())
                {
                    var element = item.Value.ToObject<T>();
                    if (element is IKeyed keyed)
                    {
                        keyed.Key = item.Name;
                    }
                    list.Add(element);
                }
            }
            return list;
        }
    }
}
