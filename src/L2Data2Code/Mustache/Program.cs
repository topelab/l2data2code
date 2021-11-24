using L2Data2Code.BaseMustache.Extensions;
using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.BaseMustache.Services;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Mustache
{
    class Program
    {
        private static IMustacheRenderizer mustacher;
        static void Main(string[] args)
        {
            var options = MustacheOptionsFactory.Create(args);
            IJsonSetting jsonSetting = new JsonSetting(options.JsonDataFile);
            IMustacheHelpers helpers = new MustacheHelpers();
            mustacher = new MustacheRenderizer(helpers);

            IFileExecutor fileExecutor = new FileExecutor(options.TemplatePath);

            fileExecutor.Run(
                (path) => CreatePaths(path, jsonSetting.Config, options.TemplatePath, options.OutputPath, options.Collection),
                (file) => MustacheToOutput(file, jsonSetting.Config, options.TemplatePath, options.OutputPath, options.Collection));
        }

        private static void CreatePaths(string path, JObject view, string sourcePath, string outputPath, string entity)
        {
            outputPath = outputPath.AddPathSeparator();
            sourcePath = sourcePath.AddPathSeparator();
            var entities = string.IsNullOrWhiteSpace(entity) ? view : view.SelectToken(entity);

            if (entities != null)
            {
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }


                foreach (var item in entities)
                {
                    string destPath = mustacher.Render(path.Replace(sourcePath, outputPath, StringComparison.CurrentCultureIgnoreCase), item);
                    if (!Directory.Exists(destPath))
                    {
                        Directory.CreateDirectory(destPath);
                    }
                }
            }

        }

        private static void MustacheToOutput(string file, JObject view, string sourcePath, string outputPath, string entity)
        {
            outputPath = outputPath.AddPathSeparator();
            sourcePath = sourcePath.AddPathSeparator();
            var entities = string.IsNullOrWhiteSpace(entity) ? view : view.SelectToken(entity);

            if (entities != null)
            {
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                foreach (var item in entities)
                {
                    string outpuFile = mustacher.Render(file.Replace(sourcePath, outputPath, StringComparison.CurrentCultureIgnoreCase), item);
                    string text = mustacher.Render(File.ReadAllText(file), item);
                    File.WriteAllText(outpuFile, text);
                    Console.WriteLine(outpuFile);
                }
            }
        }
    }
}
