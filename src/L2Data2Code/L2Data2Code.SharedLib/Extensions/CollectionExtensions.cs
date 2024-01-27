using System.Collections.Specialized;
using System.Linq;

namespace L2Data2Code.SharedLib.Extensions
{
    public static class CollectionExtensions
    {
        public static string ToSemiColonSeparatedString(this NameValueCollection nameValueCollection) =>
            nameValueCollection == null ? null : string.Join(';', nameValueCollection.AllKeys.Select(k => $"{k}={nameValueCollection[k]}")) + ";";

        public static string ToNestedSemiColonSeparatedString(this NameValueCollection nameValueCollection)
        {
            return nameValueCollection == null ? null : string.Join(';', nameValueCollection.AllKeys.Select((k, i) => $"{(i > 0 ? " ." : string.Empty)}{k}={nameValueCollection[k]}")) + ";";
        }
    }
}
