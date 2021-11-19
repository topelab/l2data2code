using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.SharedLib.Extensions;
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
            Register("FormatCurrency", (HelperContext context, decimal number) => number.ToString("C"));
            Register("GetVar", (HelperContext context, string key) => keyValuePairs.TryGetValue(key, out var value) ? value : null);
            Register("Or", (HelperContext context, string cond1, string cond2, string result, string otherCase) => cond1.IsTrue() || cond2.IsTrue() ? result : otherCase);
            Register("And", (HelperContext context, string cond1, string cond2, string result, string otherCase) => cond1.IsTrue() && cond2.IsTrue() ? result : otherCase);
            Register("Equal", (HelperContext context, string cond1, string cond2, string result, string otherCase) => cond1.Equals(cond2) ? result : otherCase);
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