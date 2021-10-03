using L2Data2Code.SharedLib.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace L2Data2Code.BaseGenerator.Entities
{
    public class FileService
    {
        private static readonly Encoding defaultEncoding = new UTF8Encoding(false);
        private static Encoding encoding = defaultEncoding;
        private static string endOfLine = Environment.NewLine;

        private static readonly Dictionary<string, Encoding> defaultEncodings = new()
        {
            { "utf8", new UTF8Encoding(false) },
            { "latin1", Encoding.Latin1 },
            { "utf8-bom", new UTF8Encoding(true) }
        };

        private static readonly Dictionary<string, string> defaultEndingLines = new()
        {
            { "crlf", "\r\n" },
            { "lf", "\n" }
        };

        public static string Read(string file)
        {
            return File.ReadAllText(file).ReplaceEndOfLine("\n");
        }

        public static string Read(string file, Func<string, string> conversionFunc)
        {
            string content = Read(file);
            return conversionFunc(content);
        }

        public static void Write(string file, string content)
        {
            File.WriteAllText(file, content.ReplaceEndOfLine(endOfLine), encoding);
        }

        public static void ResetSettings()
        {
            encoding = defaultEncoding;
            endOfLine = Environment.NewLine;
        }

        public static void SetSettings(Encoding encoding, string endOfLine)
        {
            FileService.encoding = encoding;
            FileService.endOfLine = endOfLine;
        }

        public static void SetSettings(string encodingKey, string endOfLineKey)
        {
            encoding = defaultEncodings.ContainsKey(encodingKey) ? defaultEncodings[encodingKey] : Encoding.GetEncoding(encodingKey);
            endOfLine = defaultEndingLines.ContainsKey(endOfLineKey) ? defaultEndingLines[endOfLineKey] : Environment.NewLine;
        }

    }
}
