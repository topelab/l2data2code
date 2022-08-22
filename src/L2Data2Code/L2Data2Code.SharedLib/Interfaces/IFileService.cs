using System;
using System.Collections.Generic;

namespace L2Data2Code.SharedLib.Interfaces
{
    public interface IFileService
    {
        Dictionary<string, string> GetPartials(string basePath, string partialsPath = null);
        string Read(string file);
        string Read(string file, Func<string, string> conversionFunc);
        string ReadWithIncludes(string templateFile, string basePath = null);
        void Initialize(string encodingKey, string endOfLineKey);
        void Write(string file, string content);
    }
}