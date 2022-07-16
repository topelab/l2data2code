namespace L2Data2Code.BaseMustache.Interfaces
{
    public interface IPathRenderizer
    {
        string TemplateFileName<T>(string templatesPath, string filePath, T replacement) where T : class;
    }
}