using CommandLine;
using L2Data2Code.SchemaReader.Interface;
using Topelab.Core.Resolver.Microsoft;


namespace Schema2Json
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Proceed);
        }

        private static void Proceed(Options options)
        {
            var resolver = ResolverFactory.Create(SetupDI.Register());
            var factory = resolver.Get<ISchema2JsonFactory>();
            factory.Create(options.OutputPath, options.Schema);
        }
    }
}