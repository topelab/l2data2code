using System;
using System.IO;

namespace L2Data2Code.CLIBase.Options
{
    public class CLIOptionsFactory
    {
        public static ICLIOptions Create(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                throw new Exception($"Needs {nameof(ICLIOptions.JsonDataFile)} & {nameof(ICLIOptions.TemplatePath)}");
            }
            return new CLIOptions
            {
                JsonDataFile = args[0],
                TemplatePath = args[1],
                OutputPath = args.Length > 2 ? args[2] : Path.Combine(AppContext.BaseDirectory, "output"),
                Collection = args.Length > 3 ? args[3] : null
            };
        }
    }
}
