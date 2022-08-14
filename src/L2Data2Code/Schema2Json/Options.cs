using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Schema2Json
{
    internal class Options
    {
        [Option('o', "output", Required = true, HelpText = "Set output path")]
        public string OutputPath { get; set; }

        [Option('s', "schema", Required = true, HelpText = "Set schema name to be read")]
        public string Schema { get; set; }

        [Usage(ApplicationAlias = "Schema2Json")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Normal scenario", new Options { OutputPath = @"c:\arc\dl", Schema = "demo" });
            }
        }
    }
}
