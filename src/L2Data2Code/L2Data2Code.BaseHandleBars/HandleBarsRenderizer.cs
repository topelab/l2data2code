using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.Enums;
using HandlebarsDotNet.Helpers.Helpers;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Interfaces;
using System.Collections.Generic;

namespace L2Data2Code.BaseHandleBars
{
    public class HandleBarsRenderizer : IMustacheRenderizer
    {
        private readonly Dictionary<string, object> values;
        private readonly Dictionary<string, IHelpers> helpers;
        private readonly Dictionary<int, HandlebarsTemplate<object, object>> templateCache;
        private readonly IHandlebars handlebars;
        private const string separator = "!!!PS!!!";

        public bool CanRenderParentInsideChild { get; private set; } = false;

        public HandleBarsRenderizer()
        {
            values = new Dictionary<string, object>();
            helpers = new Dictionary<string, IHelpers>
            {
                { "Custom", new CustomHelpers(handlebars, values) }
            };

            templateCache = new Dictionary<int, HandlebarsTemplate<object, object>>();
            handlebars = Handlebars.CreateSharedEnvironment();
            handlebars.Configuration.AliasProviders.Add(AliasProviderFactory.Create());
            HandlebarsHelpers.Register(handlebars, options => { options.UseCategoryPrefix = true; });
            HandlebarsHelpers.Register(handlebars, options => { options.UseCategoryPrefix = false; options.Categories = new[] { (Category)999 }; options.CustomHelpers = helpers; });
        }

        public string Render(string template, object view)
        {
            var newValues = view as Dictionary<string, object>;
            if (newValues != null)
            {
                values.ClearAndAddRange(newValues);
            }
            var key = Compile(template);
            return Run(key, newValues ?? view);
        }

        public string RenderPath(string template, object view)
        {
            return Render(template.Replace("\\", separator), view).Replace(separator, "\\");
        }

        public void SetupPartials(Dictionary<string, string> partialsFiles)
        {
            foreach (var partialName in partialsFiles.Keys)
            {
                handlebars.RegisterTemplate(partialName, partialsFiles[partialName]);
            }
        }

        private int Compile(string template, int? key = null)
        {
            key ??= template.GetHashCode();
            if (!templateCache.ContainsKey(key.Value))
            {
                templateCache[key.Value] = handlebars.Compile(template);
            }
            return key.Value;
        }

        private string Run(int key, object context) => templateCache[key](context);

    }
}
