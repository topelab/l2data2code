using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Data2Code.SharedLib.Extensions
{
    public static class CollectionExtensions
    {
        public static string ToSemiColonSeparatedString(this NameValueCollection nameValueCollection) =>
            nameValueCollection == null ? null : string.Join(';', nameValueCollection.AllKeys.Select(k => $"{k}={nameValueCollection[k]}")) + ";";
    }
}
