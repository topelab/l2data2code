using L2Data2Code.SharedLib.Helpers;

namespace Mustache
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = MustacheOptionsFactory.Create(args);
            Resolver.Initialize(SetupDI.Register());
            var mustacheAction = Resolver.Get<IMustacheAction>();

            mustacheAction.Initialize(options);
            mustacheAction.Run();
        }

    }
}
