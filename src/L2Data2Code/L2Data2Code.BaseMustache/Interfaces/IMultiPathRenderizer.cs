using System.Collections.Generic;

namespace L2Data2Code.BaseMustache.Interfaces
{
    public interface IMultiPathRenderizer
    {
        bool CanApplyMultiPath(string filePath);
        Dictionary<string, string> ApplyMultiPath<T>(string filePath, string originalText, T replacement) where T : class;
    }
}