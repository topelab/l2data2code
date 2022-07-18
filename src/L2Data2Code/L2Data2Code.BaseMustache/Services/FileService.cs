using L2Data2Code.SharedLib.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace L2Data2Code.BaseMustache.Services
{
    public class FileService : IFileService
    {
        private const string INCLUDE_TEXT = "{{!include ";
        private readonly Regex includeRegex = new(@"\{\{!include\s+(?<name>[^\}]+)\}\}", RegexOptions.Singleline | RegexOptions.Compiled);
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
            { "lf", "\n" }
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

        public void SetSettings(Encoding encoding, string endOfLine)
        {
            this.encoding = encoding;
            this.endOfLine = endOfLine;
        }

        public void SetSettings(string encodingKey, string endOfLineKey)
        {
            encoding = defaultEncodings.ContainsKey(encodingKey) ? defaultEncodings[encodingKey] : Encoding.GetEncoding(encodingKey);
            endOfLine = defaultEndingLines.ContainsKey(endOfLineKey) ? defaultEndingLines[endOfLineKey] : Environment.NewLine;
        }

        public string ReadWithIncludes(string templateFile, string basePath = null)
            => ReadWithIncludes(templateFile, basePath, null);

        private string ReadWithIncludes(string templateFile, string basePath = null, HashSet<string> includedFiles = null)
        {
            includedFiles ??= new HashSet<string>();

            if (includedFiles.Contains(templateFile))
            {
                return null;
            }

            includedFiles.Add(templateFile);

            basePath ??= Path.GetDirectoryName(templateFile);
            var templateContent = Read(templateFile);
            if (templateContent.Contains(INCLUDE_TEXT))
            {
                var lines = templateContent.Split('\n');
                List<string> replacedLines = new();
                foreach (var line in lines)
                {
                    if (line.StartsWith(INCLUDE_TEXT))
                    {
                        var match = includeRegex.Match(line);
                        if (match.Success)
                        {
                            var file = match.Groups["name"].Value.Replace('/', Path.DirectorySeparatorChar);
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
                                    replacedLines.Add(fileContent.ReplaceEndOfLine("\n"));
                                }
                            }
                            continue;
                        }
                    }
                    replacedLines.Add(line);
                }
                templateContent = string.Join("\n", replacedLines);
            }

            return templateContent;
        }
    }
}
