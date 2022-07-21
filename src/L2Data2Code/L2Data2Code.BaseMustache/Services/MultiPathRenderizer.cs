using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.SharedLib.Extensions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace L2Data2Code.BaseMustache.Services
{
    public class MultiPathRenderizer : IMultiPathRenderizer
    {
        private static readonly Regex templateMultipleFiles = new(@"(?<start>^|\\)foreach\(+(?<key>[^\)]+)\)(?<name>.+)$", RegexOptions.Singleline | RegexOptions.Compiled);

        private const string foreachTag = "foreach(";
        private const string startTag = "{{";
        private const string endTag = "}}";
        private const string fileSeparator = "<<<END>>>";
        private readonly IMustacheRenderizer mustacheRenderizer;

        public MultiPathRenderizer(IMustacheRenderizer mustacheRenderizer)
        {
            this.mustacheRenderizer = mustacheRenderizer ?? throw new ArgumentNullException(nameof(mustacheRenderizer));
        }

        public bool CanApplyMultiPath(string filePath) => filePath.Contains(foreachTag);

        public Dictionary<string, string> ApplyMultiPath<T>(string filePath, string originalText, T replacement) where T : class
        {
            Dictionary<string, string> files = new();
            if (filePath.Contains(foreachTag))
            {
                string tag = null;
                var match = templateMultipleFiles.Match(filePath);
                if (match.Success)
                {
                    filePath = templateMultipleFiles.Replace(filePath, "${start}${name}");
                    tag = match.Groups["key"].Value;
                }

                var fileNames = GetReplacements(tag, filePath, replacement);
                var contents = GetReplacements(tag, originalText, replacement);

                for (var i = 0; i < fileNames.Length; i++)
                {
                    if (fileNames[i].NotEmpty())
                    {
                        files.Add(fileNames[i], contents[i]);
                    }
                }
            }
            return files;
        }

        private string[] GetReplacements<T>(string tag, string originalText, T replacement)
        {
            var toReplace = $"{startTag}#{tag}{endTag}{originalText}{fileSeparator}{startTag}/{tag}{endTag}";
            var replaced = mustacheRenderizer.Render(toReplace, replacement);
            return replaced.Split(fileSeparator)[..^1];
        }
    }
}
