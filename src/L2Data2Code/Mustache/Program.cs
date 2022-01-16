using Topelab.Core.Resolver.Unity;

namespace Mustache
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = MustacheOptionsFactory.Create(args);
            var resolver = ResolverFactory.Create(SetupDI.Register());
            var mustacheAction = resolver.Get<IMustacheAction>();

            mustacheAction.Initialize(options);
            mustacheAction.Run();
        }

    }
}
