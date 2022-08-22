using System.Collections.Generic;
using System.Linq;

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

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dest, Dictionary<TKey, TValue> source)
        {
            foreach (var item in source)
            {
                dest[item.Key] = item.Value;
            }
        }

        public static void ClearAndAddRange<TKey, TValue>(this Dictionary<TKey, TValue> dest, Dictionary<TKey, TValue> source)
        {
            dest.Clear();
            dest.AddRange(source);
        }
    }
}
