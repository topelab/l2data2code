using System.Collections.Generic;

namespace L2Data2Code.SharedLib.Interfaces
{
    public interface IMustacheRenderizer
    {
        bool CanRenderParentInsideChild { get; }

        string Render(string template, object view);
        string RenderPath(string template, object view);
        void SetupPartials(Dictionary<string, string> partialsFiles);
    }
}