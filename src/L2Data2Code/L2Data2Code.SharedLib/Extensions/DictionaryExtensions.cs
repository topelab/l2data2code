using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Data2Code.SharedLib.Extensions
{
    public static class DictionaryExtensions
    {
        public static string All(this Dictionary<string, string> keyValuePairs)
        {
            return string.Join(", ", keyValuePairs.Select(d => $"{d.Key}: {d.Value}"));
        }

        public static string[] Keys(this Dictionary<string, string> keyValuePairs)
        {
            return keyValuePairs.Select(d => d.Key).ToArray();
        }
    }
}
