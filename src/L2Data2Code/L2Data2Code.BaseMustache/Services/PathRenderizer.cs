using L2Data2Code.BaseMustache.Interfaces;
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

        public string TemplateFileName<T>(string templatesPath, string filePath, T replacement) where T : class
        {
            var partialName = filePath.Replace(templatesPath, "");

            var iwhen = partialName.IndexOf(conditionalFileName);
            while (iwhen >= 0)
            {
                var itag = iwhen + conditionalFileName.Length;
                var ewhen = partialName.IndexOf(endOfConditionalFileName, itag);
                var tag = partialName[itag..ewhen];
                if (!CheckTemplateName(tag, replacement))
                    return null;
                partialName = string.Concat(partialName[..iwhen], partialName[(ewhen + endOfConditionalFileName.Length)..]);
                iwhen = partialName.IndexOf(conditionalFileName);
            }

            partialName = mustacheRenderizer.Render(partialName, replacement);

            return partialName;
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
            var result = false;

            var expression = tag.TrimStart('^').Split('=');
            var key = expression[0].Trim();
            var value = expression[1].Trim();

            if (replacement is IDictionary<string, object> dictionary)
            {
                if (dictionary.ContainsKey(key) && dictionary[key].ToString() == value)
                {
                    result = true;
                }
            }
            else
            {
                var replacementValue = typeof(T).GetProperty(key).GetValue(replacement, null)?.ToString();
                result = replacementValue == value;
            }

            if (tag.StartsWith("^"))
                return !result;

            return result;
        }
    }
}
