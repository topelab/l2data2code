using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace L2Data2Code.SharedLib.Services
{
    public class MultiPathRenderizer : IMultiPathRenderizer
    {
        private static readonly Regex templateMultipleFiles = new(@"(?<start>.+)foreach\(+(?<tag>[^\)]+)\)(?<name>.+)$", RegexOptions.Singleline | RegexOptions.Compiled);

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
                var match = templateMultipleFiles.Match(filePath);
                if (match.Success)
                {
                    var start = match.Groups["start"].Value;
                    var tag = match.Groups["tag"].Value;
                    var name = match.Groups["name"].Value;

                    var fileNames = GetReplacements(tag, name, replacement, start);
                    var contents = GetReplacements(tag, originalText, replacement);

                    for (var i = 0; i < fileNames.Length; i++)
                    {
                        if (fileNames[i].NotEmpty())
                        {
                            files.Add(fileNames[i], contents[i]);
                        }
                    }
                }
            }
            return files;
        }

        private string[] GetReplacements<T>(string tag, string originalText, T replacement, string start = null)
        {
            if (!mustacheRenderizer.CanRenderParentInsideChild && start != null)
            {
                start = start.Replace(startTag, $"{startTag}../");
            }
            var toReplace = $"{startTag}#{tag}{endTag}{start}{originalText}{fileSeparator}{startTag}/{tag}{endTag}";
            var replaced = mustacheRenderizer.RenderPath(toReplace, replacement);
            return replaced.Split(fileSeparator)[..^1];
        }
    }
}
