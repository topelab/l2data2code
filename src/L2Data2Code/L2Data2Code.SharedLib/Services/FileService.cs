using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace L2Data2Code.SharedLib.Services
{
    public class FileService : IFileService
    {
        private const string INCLUDE_TEXT = "{{!include ";
        private readonly Regex includeRegex = new(@"(?<spaces>\s*)\{\{!include\s+(?<name>[^\}]+)\}\}", RegexOptions.Singleline | RegexOptions.Compiled);
        private readonly Encoding defaultEncoding = new UTF8Encoding(false);

        private Encoding encoding;
        private string endOfLine = Environment.NewLine;

        public FileService()
        {
            encoding = defaultEncoding;
            endOfLine = Environment.NewLine;
        }

        private readonly Dictionary<string, Encoding> defaultEncodings = new()
        {
            { "utf8", new UTF8Encoding(false) },
            { "latin1", Encoding.Latin1 },
            { "utf8-bom", new UTF8Encoding(true) }
        };

        private readonly Dictionary<string, string> defaultEndingLines = new()
        {
            { "crlf", "\r\n" },
            { "lf", "\n" },
            { "cr", "\r" },
        };

        public string Read(string file)
        {
            return File.ReadAllText(file).ReplaceEndOfLine("\n");
        }

        public string Read(string file, Func<string, string> conversionFunc)
        {
            var content = Read(file);
            return conversionFunc(content);
        }

        public void Write(string file, string content)
        {
            File.WriteAllText(file, content.ReplaceEndOfLine(endOfLine), encoding);
        }

        public void Initialize(string encodingKey, string endOfLineKey)
        {
            encoding = defaultEncodings.ContainsKey(encodingKey) ? defaultEncodings[encodingKey] : Encoding.GetEncoding(encodingKey);
            endOfLine = defaultEndingLines.ContainsKey(endOfLineKey) ? defaultEndingLines[endOfLineKey] : Environment.NewLine;
        }

        public string ReadWithIncludes(string templateFile, string basePath = null)
            => ReadWithIncludes(templateFile, basePath, null);


        public Dictionary<string, string> GetPartials(string basePath, string partialsPath = null)
        {
            var result = new Dictionary<string, string>();
            var path = Path.Combine(basePath, partialsPath ?? "partials");

            if (!string.IsNullOrWhiteSpace(partialsPath) && Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "*.partial", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    if (!result.ContainsKey(name))
                    {
                        result[name] = File.ReadAllText(file);
                    }
                }
            }

            return result;
        }

        private string ReadWithIncludes(string templateFile, string basePath = null, Dictionary<string, string> includedFiles = null)
        {
            includedFiles ??= new Dictionary<string, string>();

            basePath ??= Path.GetDirectoryName(templateFile);
            var templateContent = Read(templateFile);
            if (templateContent.Contains(INCLUDE_TEXT))
            {
                if (includedFiles.ContainsKey(templateFile))
                {
                    templateContent = includedFiles[templateFile];
                }
                else
                {
                    includedFiles.Add(templateFile, templateContent);
                    templateContent = GetReplacedContent(basePath, includedFiles, templateContent);
                    includedFiles[templateFile] = templateContent;
                }
            }

            return templateContent;
        }

        private string GetReplacedContent(string basePath, Dictionary<string, string> includedFiles, string templateContent)
        {
            var lines = templateContent.Split('\n');
            List<string> replacedLines = new();
            foreach (var line in lines)
            {
                if (line.Contains(INCLUDE_TEXT))
                {
                    var match = includeRegex.Match(line);
                    if (match.Success)
                    {
                        var file = match.Groups["name"].Value.Replace('/', Path.DirectorySeparatorChar);
                        var spaces = match.Groups["spaces"].Value;
                        var filePath = Path.Combine(basePath, file);
                        if (file.StartsWith("~\\"))
                        {
                            filePath = file.Replace("~", Directory.GetParent(basePath).Parent.FullName);
                        }
                        if (File.Exists(filePath))
                        {
                            var fileContent = ReadWithIncludes(filePath, basePath, includedFiles);
                            if (fileContent != null)
                            {
                                replacedLines.Add(AddIndent(spaces, fileContent));
                            }
                        }
                        continue;
                    }
                }
                replacedLines.Add(line);
            }
            templateContent = string.Join("\n", replacedLines);
            return templateContent;
        }

        private static string AddIndent(string spaces, string text)
        {
            var lines = text.ReplaceEndOfLine("\n").Split("\n").Select(l => string.Concat(spaces, l));
            return string.Join("\n", lines);
        }
    }
}
