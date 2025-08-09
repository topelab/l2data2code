using CommandLine;
using L2Data2Code.SchemaReader.Interface;
using System;
using Topelab.Core.Resolver.Microsoft;


namespace Scheme2Json
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
            if (!factory.IsValidSchema(options.Schema, out var error))
            {
                Console.WriteLine($"Error: {error}");
                return;
            }
            factory.Create(options.OutputPath, options.Schema);
            Console.WriteLine($"Generated JSON for schema {options.Schema}");
        }
    }
}