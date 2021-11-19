using System.Collections.Generic;

namespace L2Data2Code.BaseMustache.Extensions
{
    public static class DictionaryExtensions
    {
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
