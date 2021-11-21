using L2Data2Code.BaseMustache.Extensions;
using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.SharedLib.Extensions;
using Stubble.Extensions.JsonNet;
using Stubble.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace L2Data2Code.BaseMustache.Services
{
    public class MustacheRenderizer : IMustacheRenderizer
    {
        private static Dictionary<string, MethodInfo> _methods;
        private readonly Stubble.Core.StubbleVisitorRenderer _mustache;
        private readonly Stubble.Core.Settings.RenderSettings _settings;
        private readonly IMustacheHelpers _helpers;

        public MustacheRenderizer(IMustacheHelpers mustacheHelpers)
        {
            if (_methods == null)
            {
                _methods = new Dictionary<string, MethodInfo>();
                List<System.Type> types = Assembly.GetCallingAssembly().GetTypes().ToList();
                types.AddRange(Assembly.GetExecutingAssembly().GetTypes());
                types.AddRange(Assembly.GetAssembly(typeof(StringExtensions)).GetTypes());

                foreach (var type in types)
                {
                    foreach (var method in type.GetMethods())
                    {
                        var methodParams = method.GetParameters();

                        var IsMethodOk = method.IsStatic
                            && methodParams.Length == 1
                            && methodParams[0].ParameterType == typeof(string);

                        if (IsMethodOk)
                        {
                            if (!_methods.ContainsKey(method.Name))
                            {
                                _methods.Add(method.Name, method);
                            }
                        }
                    }
                }
            }
            _helpers = mustacheHelpers;

            _settings = new Stubble.Core.Settings.RenderSettings()
            {
                SkipHtmlEncoding = true,
            };

            _mustache = new Stubble.Core.Builders.StubbleBuilder()
                .Configure(s => s
                    .AddValueGetter(typeof(string), (o, k, i) =>
                        {
                            var isMethodOk = _methods.TryGetValue(k, out var method) && method.GetParameters()[0].ParameterType == o.GetType();
                            return !isMethodOk ? string.Empty : method.Invoke(null, new[] { o });
                        })
                    .AddJsonNet()
                    .AddHelpers((Helpers)_helpers)
                ).Build();
        }

        public string Render(string template, object view)
        {
            if (template.Contains("{{"))
            {
                var result = template.Replace("+{", "(***OPEN***)").Replace("+}", "(***CLOSE***)");
                _helpers.SetDictionary(view as IDictionary<string, object>);
                return _mustache.Render(result, view, _settings).Replace("(***OPEN***)", "{").Replace("(***CLOSE***)", "}");
            }
            return template;
        }
    }
}
