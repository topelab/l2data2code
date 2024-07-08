using System;
using System.IO;

namespace L2Data2Code.BaseGenerator.Entities
{
    /// <summary>
    /// Class that represent result for a replacement
    /// </summary>
    public class ReplacementResult
    {
        private string content;
        private readonly Func<string> func;

        public ReplacementResult(string title, string filename, string relativePath, Func<string> func)
        {
            Title = title;
            FileName = filename;
            RelativePath = relativePath;
            this.func = func;
            IsBinaryFile = Path.GetFileName(filename).StartsWith('!');
        }

        public string Title { get; private set; }
        public string FileName { get; private set; }
        public string Content => content ??= func?.Invoke() ?? string.Empty;
        public string RelativePath { get; set; }
        public bool IsBinaryFile {  get; private set; }
    }

}
