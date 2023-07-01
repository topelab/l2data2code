using System.Collections.Generic;

namespace L2Data2Code.Avalonia.Main.Vars
{
    internal interface IVarsFactory
    {
        VarsWindow Create(Dictionary<string, object> vars);
    }
}