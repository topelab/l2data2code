using L2Data2Code.SharedContext.Main.Vars;
using L2Data2CodeUI.Shared.Localize;
using System.Collections.Generic;

namespace L2Data2Code.Avalonia.Main.Vars
{
    internal class VarsFactory : IVarsFactory
    {
        public VarsWindow Create(Dictionary<string, object> vars)
        {
            VarsVM varsVM = new(vars);
            VarsWindow window = new()
            {
                DataContext = varsVM,
                Title = Strings.VarsTitle
            };
            return window;
        }
    }
}
