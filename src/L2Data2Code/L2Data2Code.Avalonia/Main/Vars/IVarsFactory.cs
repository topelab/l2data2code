using System.Collections.Generic;

namespace L2Data2Code.Main.Vars
{
    internal interface IVarsFactory
    {
        VarsWindow Create(Dictionary<string, object> vars);
    }
}