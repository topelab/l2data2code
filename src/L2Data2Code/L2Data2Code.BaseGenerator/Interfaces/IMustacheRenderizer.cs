namespace L2Data2Code.BaseGenerator.Interfaces
{
    public interface IMustacheRenderizer
    {
        string Render(string template, object view);
        void SetIsoLanguaje(string isoLang);
    }
}