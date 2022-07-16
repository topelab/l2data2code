using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.SharedLib.Extensions;
using Newtonsoft.Json.Linq;
using Stubble.Helpers;
using System.Collections.Generic;

namespace L2Data2Code.BaseMustache.Extensions
{
    public class MustacheHelpers : Helpers, IMustacheHelpers
    {
        private IDictionary<string, object> keyValuePairs;

        public MustacheHelpers()
        {
            keyValuePairs = new Dictionary<string, object>();
            Register<decimal>("FormatCurrency", (context, number) => number.ToString("C"));
            Register<string>("GetVar", (context, key) => keyValuePairs.TryGetValue(key, out var value) ? value : null);
            Register<string, string, string, string>("Or", (context, cond1, cond2, result, otherCase) => cond1.IsTrue() || cond2.IsTrue() ? result : otherCase);
            Register<string, string, string, string>("And", (context, cond1, cond2, result, otherCase) => cond1.IsTrue() && cond2.IsTrue() ? result : otherCase);
            Register<string, string, string, string>("Equal", (context, cond1, cond2, result, otherCase) => cond1.Equals(cond2) ? result : otherCase);
            Register<JArray, string>("Join", (context, items, separator) => string.Join(separator, items.Values<string>()));
            Register<JArray, string, string>("JoinWithHeader", (context, items, separator, header) => string.Concat(header, string.Join(separator, items.Values<string>())));
            Register<JArray, string, string, string>("JoinWithHeaderFooter", (context, items, separator, header, footer) => string.Concat(header, string.Join(separator, items.Values<string>()), footer));
        }

        public void SetDictionary(IDictionary<string, object> keyValuePairs)
        {
            if (keyValuePairs != null)
            {
                this.keyValuePairs = keyValuePairs;
            }
        }
    }
}