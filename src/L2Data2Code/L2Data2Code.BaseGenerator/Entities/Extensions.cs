namespace L2Data2Code.BaseGenerator.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Extensions
    {
        public static bool None<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            return !collection.Any(predicate); // ⟷ return collection.All(t => !predicate(t));
        }

        public static IEnumerable<R> Select<T, R>(this IEnumerable<T> collection, Func<T, int, bool, bool, R> selector)
        {
            var isFirst = true;
            var i = 0;
            var e = collection.GetEnumerator();
            var has1 = e.MoveNext();
            var current = has1 ? e.Current : default;
            var hasOther = e.MoveNext();
            var next = hasOther ? e.Current : default;
            while (has1)
            {
                var result = selector(current, i, isFirst, !hasOther);
                yield return result;
                ++i;
                isFirst = false;
                has1 = hasOther;
                current = next;
                hasOther = e.MoveNext();
                next = hasOther ? e.Current : default;
            }
        }


    }
}
