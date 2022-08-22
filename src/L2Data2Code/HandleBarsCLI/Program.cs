using L2Data2Code.CLIBase.Interfaces;
using L2Data2Code.CLIBase.Options;
using Topelab.Core.Resolver.Microsoft;

namespace HandleBarsCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = CLIOptionsFactory.Create(args);
            var resolver = ResolverFactory.Create(SetupDI.Register());
            var HandleBarsAction = resolver.Get<ICLIAction>();

            HandleBarsAction.Initialize(options);
            HandleBarsAction.Run();
        }

    }
}
