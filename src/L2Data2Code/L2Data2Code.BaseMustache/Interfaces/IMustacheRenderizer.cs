namespace L2Data2Code.BaseMustache.Interfaces
{
    public interface IMustacheRenderizer
    {
        string Render(string template, object view);
    }
}