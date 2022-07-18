using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.SharedLib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace L2Data2Code.BaseMustache.Services
{
    public class PathRenderizer : IPathRenderizer
    {
        private const string conditionalFileName = "when{{";
        private const string endOfConditionalFileName = "}}";
        private readonly IMustacheRenderizer mustacheRenderizer;

        public PathRenderizer(IMustacheRenderizer mustacheRenderizer)
        {
            this.mustacheRenderizer = mustacheRenderizer ?? throw new ArgumentNullException(nameof(mustacheRenderizer));
        }

        public bool TryGetFileName<T>(string filePath, T replacement, out string fileName) where T : class
            => TryGetFileName(null, filePath, replacement, out fileName);

        public bool TryGetFileName<T>(string templatesPath, string filePath, T replacement, out string fileName) where T : class
        {
            var partialName = templatesPath.IsEmpty() ? filePath : filePath.Replace(templatesPath, "");

            var iwhen = partialName.IndexOf(conditionalFileName);
            while (iwhen >= 0)
            {
                var itag = iwhen + conditionalFileName.Length;
                var ewhen = partialName.IndexOf(endOfConditionalFileName, itag);
                var tag = partialName[itag..ewhen];
                if (!CheckTemplateName(tag, replacement))
                {
                    fileName = null;
                    return false;
                }
                partialName = string.Concat(partialName[..iwhen], partialName[(ewhen + endOfConditionalFileName.Length)..]);
                iwhen = partialName.IndexOf(conditionalFileName);
            }

            fileName = mustacheRenderizer.Render(partialName, replacement);
            return true;
        }

        private static bool CheckTemplateName<T>(string tag, T replacement) where T : class
        {
            return tag.Contains('=') ? CheckTemplateNameConditionIsTrue(tag, replacement) : CheckTemplateNameIsTrue(tag, replacement);
        }

        private static bool CheckTemplateNameIsTrue<T>(string tag, T replacement) where T : class
        {
            var result = false;

            var type = typeof(T);
            var prop = type.GetProperty(tag.TrimStart('^'), typeof(bool));
            if (prop != null)
                result = (bool)prop.GetValue(replacement, null);

            if (tag.StartsWith("^"))
                return !result;

            return result;
        }

        private static bool CheckTemplateNameConditionIsTrue<T>(string tag, T replacement) where T : class
        {
            var expression = tag.TrimStart('^').Split('=');
            var key = expression[0].Trim();
            var value = expression[1].Trim();

            bool result;
            switch (replacement)
            {
                case IDictionary<string, object> dictionary:
                    result = dictionary.ContainsKey(key) && dictionary[key].ToString() == value;
                    break;
                case JToken token:
                    result = token[key]?.ToString().ToLower() == value?.ToLower();
                    break;
                default:
                    result = typeof(T).GetProperty(key).GetValue(replacement, null)?.ToString() == value;
                    break;
            }

            if (tag.StartsWith("^"))
                return !result;

            return result;
        }
    }
}
