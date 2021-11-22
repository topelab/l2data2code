using System;
using System.IO;

namespace Mustache
{
    internal class MustacheOptionsFactory
    {
        internal static MustacheOptions Create(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                throw new Exception($"Needs {nameof(MustacheOptions.JsonDataFile)} & {nameof(MustacheOptions.TemplatePath)}");
            }
            return new MustacheOptions
            {
                JsonDataFile = args[0],
                TemplatePath = args[1],
                OutputPath = args.Length > 2 ? args[2] : Path.Combine(AppContext.BaseDirectory, "output"),
                Collection = args.Length > 3 ? args[3] : null
            };
        }
    }
}
