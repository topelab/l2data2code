using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace L2Data2Code.SharedLib.Extensions
{
    public static class JSonExtensions
    {
        public static NameValueCollection ToNameValueCollection(this JToken source)
        {
            NameValueCollection valueCollection = new();
            if (source == null) return valueCollection;

            foreach (JProperty item in source)
            {
                valueCollection[item.Name] = (string)item.Value;
            }
            return valueCollection;
        }
    }
}
