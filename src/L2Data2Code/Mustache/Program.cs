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
        static void Main(string[] args)
        {
            var options = MustacheOptionsFactory.Create(args);
            options.JsonSetting = new JsonSetting(options.JsonDataFile);
            options.Mustacher = new MustacheRenderizer(new MustacheHelpers());

            IFileExecutor fileExecutor = new FileExecutor(options.TemplatePath);

            fileExecutor.Run((path) => DoAction(CreatePath, path, options),
                             (file) => DoAction(TransformFile, file, options));
        }

        private static void DoAction(Action<string, string, string, IMustacheRenderizer, JToken> action, string path, MustacheOptions options)
        {
            var outputPath = options.OutputPath.AddPathSeparator();
            var sourcePath = options.TemplatePath.AddPathSeparator();
            var view = options.JsonSetting.Config;
            var entity = options.Collection;
            var mustacher = options.Mustacher;
            var entities = string.IsNullOrWhiteSpace(entity) ? view : view.SelectToken(entity);

            if (entities != null)
            {
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                foreach (var item in entities)
                {
                    action.Invoke(path, outputPath, sourcePath, mustacher, item);
                }
            }
        }

        private static void CreatePath(string path, string outputPath, string sourcePath, IMustacheRenderizer mustacher, JToken item)
        {
            var destPath = mustacher.Render(path.Replace(sourcePath, outputPath, StringComparison.CurrentCultureIgnoreCase), item);
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
        }

        private static void TransformFile(string file, string outputPath, string sourcePath, IMustacheRenderizer mustacher, JToken item)
        {
            var outpuFile = mustacher.Render(file.Replace(sourcePath, outputPath, StringComparison.CurrentCultureIgnoreCase), item);
            var text = mustacher.Render(File.ReadAllText(file), item);
            File.WriteAllText(outpuFile, text);
            Console.WriteLine(outpuFile);
        }
    }
}
