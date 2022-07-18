using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.SharedLib.Extensions;
using Newtonsoft.Json.Linq;
using Stubble.Core;
using Stubble.Core.Builders;
using Stubble.Core.Settings;
using Stubble.Extensions.JsonNet;
using Stubble.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace L2Data2Code.BaseMustache.Services
{
    public class MustacheRenderizer : IMustacheRenderizer
    {
        private static Dictionary<string, MethodInfo> methodsInfo;
        private readonly StubbleVisitorRenderer renderer;
        private readonly RenderSettings settings;
        private readonly IMustacheHelpers mustacheHelpers;

        public MustacheRenderizer(IMustacheHelpers mustacheHelpers)
        {
            this.mustacheHelpers = mustacheHelpers ?? throw new ArgumentNullException(nameof(mustacheHelpers));

            ConfigureMethods();
            settings = ConfigureSettings();
            renderer = ConfigureRenderer();
        }

        private StubbleVisitorRenderer ConfigureRenderer()
        {
            return new StubbleBuilder()
                .Configure(s => s
                    .AddValueGetter(typeof(string), (object o, string k, bool i) =>
                    {
                        var isMethodOk = methodsInfo.TryGetValue(k, out var method) && method.GetParameters()[0].ParameterType == o.GetType();
                        return !isMethodOk ? string.Empty : method.Invoke(null, new[] { o });
                    })
                    .AddValueGetter(typeof(JArray), (object o, string k, bool i) =>
                    {
                        var isMethodOk = methodsInfo.TryGetValue(k, out var method) && method.GetParameters()[0].ParameterType == o.GetType();
                        return !isMethodOk ? string.Empty : method.Invoke(null, new[] { o });
                    })
                    .AddJsonNet()
                    .AddHelpers((Helpers)mustacheHelpers)
                ).Build();
        }

        private RenderSettings ConfigureSettings() => new RenderSettings() { SkipHtmlEncoding = true };

        private static void ConfigureMethods()
        {
            if (methodsInfo == null)
            {
                methodsInfo = new Dictionary<string, MethodInfo>();
                List<Type> types = Assembly.GetCallingAssembly().GetTypes().ToList();
                types.AddRange(Assembly.GetExecutingAssembly().GetTypes());
                types.AddRange(Assembly.GetAssembly(typeof(StringExtensions)).GetTypes());
                types.AddRange(Assembly.GetAssembly(typeof(JSonExtensions)).GetTypes());

                foreach (var type in types)
                {
                    foreach (var method in type.GetMethods())
                    {
                        var methodParams = method.GetParameters();

                        var IsMethodOk = method.IsStatic
                            && methodParams.Length == 1
                            && (methodParams[0].ParameterType == typeof(string) || methodParams[0].ParameterType == typeof(JArray));

                        if (IsMethodOk)
                        {
                            if (!methodsInfo.ContainsKey(method.Name))
                            {
                                methodsInfo.Add(method.Name, method);
                            }
                        }
                    }
                }
            }
        }

        public string Render(string template, object view)
        {
            if (template.Contains("{{"))
            {
                var result = template.Replace("+{", "(***OPEN***)").Replace("+}", "(***CLOSE***)");
                mustacheHelpers.SetDictionary(view as IDictionary<string, object>);
                return renderer.Render(result, view, settings).Replace("(***OPEN***)", "{").Replace("(***CLOSE***)", "}");
            }
            return template;
        }
    }
}
