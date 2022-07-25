using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace L2Data2Code.BaseHandleBars
{
    public class HandleBarsRenderizer : IHandleBarsRenderizer
    {
        private IDictionary<string, object> values;
        private readonly Dictionary<int, HandlebarsTemplate<object, object>> templateCache;
        private readonly IHandlebars handlebars;
        private const string separator = "!!!PS!!!";

        public bool CanRenderParentInsideChild { get; private set; } = false;

        public HandleBarsRenderizer()
        {
            values = new Dictionary<string, object>();
            templateCache = new Dictionary<int, HandlebarsTemplate<object, object>>();
            handlebars = Handlebars.CreateSharedEnvironment();

            handlebars.RegisterHelper("FormatCurrency", (writer, context, parameters) => { writer.Write(parameters.At<decimal>(0).ToString("C")); });
            handlebars.RegisterHelper("GetVar", (writer, context, parameters) => { writer.Write(values.TryGetValue(parameters.At<string>(0), out var value) ? value : string.Empty); });
            handlebars.RegisterHelper("Join", (writer, context, parameters) => { writer.Write(string.Join(parameters.At<string>(1), parameters.At<JArray>(0).Values<string>())); });
            handlebars.RegisterHelper("JoinWithHeader", (writer, context, parameters) => { writer.Write(string.Concat(parameters.At<string>(2), string.Join(parameters.At<string>(1), parameters.At<JArray>(0).Values<string>()))); });
            handlebars.RegisterHelper("JoinWithHeaderFooter", (writer, context, parameters) => { writer.Write(string.Concat(parameters.At<string>(2), string.Join(parameters.At<string>(1), parameters.At<JArray>(0).Values<string>()), parameters.At<string>(3))); });

            handlebars.Configuration.AliasProviders.Add(AliasProviderFactory.Create());
            HandlebarsHelpers.Register(handlebars, options => { options.UseCategoryPrefix = true; });
        }

        public int Compile(string template, int? key = null)
        {
            key ??= template.GetHashCode();
            if (!templateCache.ContainsKey(key.Value))
            {
                templateCache[key.Value] = handlebars.Compile(template);
            }
            return key.Value;
        }

        public string Render(string template, object view)
        {
            var newValues = view as IDictionary<string, object>;
            if (newValues != null)
            {
                values = newValues;
            }
            var key = Compile(template);
            return Run(key, newValues ?? view);
        }

        public string RenderPath(string template, object view)
        {
            return Render(template.Replace("\\", separator), view).Replace(separator, "\\");
        }

        public string Run(int key, object context) => templateCache[key](context);
    }
}
