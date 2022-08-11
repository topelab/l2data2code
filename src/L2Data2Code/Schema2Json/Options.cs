using CommandLine;

namespace Schema2Json
{
    internal class Options
    {
        [Option('o', "output", Required = true, HelpText = "Set output path")]
        public string OutputPath { get; set; }

        [Option('s', "schema", Required = true, HelpText = "Set schema name to be read")]
        public string Schema { get; set; }
    }
}
