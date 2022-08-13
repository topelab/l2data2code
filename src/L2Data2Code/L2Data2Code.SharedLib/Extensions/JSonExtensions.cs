using Newtonsoft.Json.Linq;
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
    }
}
