namespace L2Data2Code.CLIBase.Options
{
    public class CLIOptions : ICLIOptions
    {
        public string JsonDataFile { get; set; }
        public string TemplatePath { get; set; }
        public string OutputPath { get; set; }
        public string Collection { get; set; }

    }
}
