namespace L2Data2Code.CLIBase.Options
{
    public interface ICLIOptions
    {
        string JsonDataFile { get; set; }
        string TemplatePath { get; set; }
        string OutputPath { get; set; }
        string Collection { get; set; }

    }
}
