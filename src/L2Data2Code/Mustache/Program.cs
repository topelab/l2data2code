using L2Data2Code.CLIBase.Interfaces;
using L2Data2Code.CLIBase.Options;
using Topelab.Core.Resolver.Microsoft;

namespace Mustache
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = CLIOptionsFactory.Create(args);
            var resolver = ResolverFactory.Create(SetupDI.Register());
            var mustacheAction = resolver.Get<ICLIAction>();

            mustacheAction.Initialize(options);
            mustacheAction.Run();
        }

    }
}
