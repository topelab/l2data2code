using Unity;
using Unity.Resolution;

namespace Mustache
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = MustacheOptionsFactory.Create(args);
            var container = SetupDI.Container;
            var mustacheAction = container.Resolve<IMustacheAction>(new ParameterOverride(nameof(options), options));
            mustacheAction.Run();
        }

    }
}
