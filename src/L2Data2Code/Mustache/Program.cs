using Unity;

namespace Mustache
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = MustacheOptionsFactory.Create(args);
            var container = SetupDI.Container;
            var mustacheAction = container.Resolve<IMustacheAction>();

            mustacheAction.Initialize(options);
            mustacheAction.Run();
        }

    }
}
