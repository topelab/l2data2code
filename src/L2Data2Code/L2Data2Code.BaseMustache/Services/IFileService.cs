using System;
using System.Text;

namespace L2Data2Code.BaseMustache.Services
{
    public interface IFileService
    {
        string Read(string file);
        string Read(string file, Func<string, string> conversionFunc);
        string ReadWithIncludes(string templateFile, string basePath = null);
        void SetSettings(Encoding encoding, string endOfLine);
        void SetSettings(string encodingKey, string endOfLineKey);
        void Write(string file, string content);
    }
}