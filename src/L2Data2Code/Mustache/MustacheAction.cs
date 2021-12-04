using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Mustache
{
    internal class MustacheAction : IMustacheAction
    {
        private readonly IMustacheRenderizer renderizer;
        private readonly IJsonSetting jsonSetting;
        private readonly IFileExecutor fileExecutor;
        private MustacheOptions options;

        public MustacheAction(IJsonSetting jsonSetting, IMustacheRenderizer renderizer, IFileExecutor fileExecutor)
        {
            this.jsonSetting = jsonSetting ?? throw new ArgumentNullException(nameof(jsonSetting));
            this.renderizer = renderizer ?? throw new ArgumentNullException(nameof(renderizer));
            this.fileExecutor = fileExecutor ?? throw new ArgumentNullException(nameof(fileExecutor));
        }

        public void Initialize(MustacheOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            jsonSetting.Initialize(options.JsonDataFile);
            fileExecutor.Initialize(options.TemplatePath);
        }

        public void Run()
        {
            fileExecutor.Run((path) => DoAction(CreatePath, path, options),
                 (file) => DoAction(TransformFile, file, options));
        }

        private void DoAction(Action<string, string, string, JToken> action, string path, MustacheOptions options)
        {
            var outputPath = options.OutputPath.AddPathSeparator();
            var sourcePath = options.TemplatePath.AddPathSeparator();
            var view = jsonSetting.Config;
            var entity = options.Collection;
            var entities = string.IsNullOrWhiteSpace(entity) ? view : view.SelectToken(entity);

            if (entities != null)
            {
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                foreach (var item in entities)
                {
                    action.Invoke(path, outputPath, sourcePath, item);
                }
            }
        }

        private void CreatePath(string path, string outputPath, string sourcePath, JToken item)
        {
            var destPath = renderizer.Render(path.Replace(sourcePath, outputPath, StringComparison.CurrentCultureIgnoreCase), item);
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
        }

        private void TransformFile(string file, string outputPath, string sourcePath, JToken item)
        {
            var outpuFile = renderizer.Render(file.Replace(sourcePath, outputPath, StringComparison.CurrentCultureIgnoreCase), item);
            var text = renderizer.Render(File.ReadAllText(file), item);
            File.WriteAllText(outpuFile, text);
            Console.WriteLine(outpuFile);
        }

    }
}
