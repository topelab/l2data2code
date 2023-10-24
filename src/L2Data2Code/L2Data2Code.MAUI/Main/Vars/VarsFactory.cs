using L2Data2Code.MAUI.Main.Vars;
using L2Data2Code.SharedContext.Main.Vars;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Localize;

namespace L2Data2Code.Main.Vars
{
    internal class VarsFactory : IVarsFactory
    {
        private readonly IGeneratorAdapter adapter;

        public VarsFactory(IGeneratorAdapter adapter)
        {
            this.adapter = adapter;
        }

        public Page Create()
        {
            VarsVM varsVM = new(adapter.CompiledVars);
            Page page = new()
            {
                BindingContext = varsVM,
                Title = Strings.VarsTitle
            };
            return page;
        }
    }
}
