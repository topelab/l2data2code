namespace L2Data2Code.BaseMustache.Interfaces
{
    public interface IPathRenderizer
    {
        bool TryGetFileName<T>(string templatesPath, string filePath, T replacement, out string fileName) where T : class;
        bool TryGetFileName<T>(string filePath, T replacement, out string fileName) where T : class;
    }
}