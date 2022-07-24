using L2Data2Code.SharedLib.Interfaces;

namespace L2Data2Code.BaseHandleBars
{
    public interface IHandleBarsRenderizer : IMustacheRenderizer
    {
        int Compile(string template, int? key = null);
        string Run(int key, object context);
    }
}