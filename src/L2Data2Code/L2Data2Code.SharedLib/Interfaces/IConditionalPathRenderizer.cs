namespace L2Data2Code.SharedLib.Interfaces
{
    public interface IConditionalPathRenderizer
    {
        bool TryGetFileName<T>(string templatesPath, string filePath, T replacement, out string fileName) where T : class;
        bool TryGetFileName<T>(string filePath, T replacement, out string fileName) where T : class;
    }
}