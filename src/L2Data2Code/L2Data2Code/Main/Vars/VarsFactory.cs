using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeWPF.Vars;

namespace L2Data2Code.Main.Vars
{
    internal class VarsFactory : IVarsFactory
    {
        private readonly IGeneratorAdapter adapter;

        public VarsFactory(IGeneratorAdapter adapter)
        {
            this.adapter = adapter;
        }

        public VarsWindow Create()
        {
            VarsVM varsVM = new(adapter.CompiledVars);
            VarsWindow window = new()
            {
                DataContext = varsVM
            };
            return window;
        }
    }
}
