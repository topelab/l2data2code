using HandlebarsDotNet;
using L2Data2Code.SharedLib.Extensions;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Reflection;

namespace L2Data2Code.BaseHandleBars
{
    internal class AliasProviderFactory
    {
        public static DelegatedMemberAliasProvider Create()
        {
            DelegatedMemberAliasProvider aliasProvider = new();

            var methods = typeof(StringExtensions)
                .GetMethods()
                .Where(m => m.IsStatic && m.GetParameters().Length == 1)
                .ToList();

            methods.ForEach(m => AddMethodInfo(aliasProvider, m));
            return aliasProvider;
        }

        private static void AddMethodInfo(DelegatedMemberAliasProvider aliasProvider, MethodInfo method)
        {
            var firstParameterType = method.GetParameters()[0].ParameterType;

            if (firstParameterType == typeof(string))
            {
                aliasProvider.AddAlias<string>(method.Name, a => method.Invoke(null, new[] { a }));
            }
            else
            {
                aliasProvider.AddAlias(typeof(object), method.Name, a => a is JArray ? string.Empty : method.Invoke(null, new[] { a.ToString() }));
            }

        }
    }
}
