using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.SharedLib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Mustache
{
    /// <summary>
    /// Run a Mustache action
    /// </summary>
    internal class MustacheAction : IMustacheAction
    {
        private readonly IMustacheRenderizer renderizer;
        private readonly IFileExecutor fileExecutor;
        private readonly IMustacheOptionsInitializer mustacheOptionsInitializer;
        private readonly IPathRenderizer pathRenderizer;
        private MustacheOptions options;
        private JToken entities;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="renderizer">Renderer</param>
        /// <param name="fileExecutor">File executor</param>
        /// <param name="mustacheOptionsInitializer">Mustache options initializer</param>
        /// <param name="pathRenderizer">Path renderer</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MustacheAction(IMustacheRenderizer renderizer, IFileExecutor fileExecutor, IMustacheOptionsInitializer mustacheOptionsInitializer, IPathRenderizer pathRenderizer)
        {
            this.renderizer = renderizer ?? throw new ArgumentNullException(nameof(renderizer));
            this.fileExecutor = fileExecutor ?? throw new ArgumentNullException(nameof(fileExecutor));
            this.mustacheOptionsInitializer = mustacheOptionsInitializer ?? throw new ArgumentNullException(nameof(mustacheOptionsInitializer));
            this.pathRenderizer = pathRenderizer ?? throw new ArgumentNullException(nameof(pathRenderizer));
        }

        /// <summary>
        /// Initialize options
        /// </summary>
        /// <param name="options">Mustache options</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Initialize(MustacheOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            fileExecutor.Initialize(options.TemplatePath);
            entities = mustacheOptionsInitializer.Initialize(options);
        }

        /// <summary>
        /// Run Mustache action
        /// </summary>
        public void Run()
        {
            if (entities != null)
            {
                fileExecutor.RunOnPaths((path) => DoAction(CreatePath, path, options));
                fileExecutor.RunOnFiles((file) => DoAction(TransformFile, file, options));
            }
        }

        private void DoAction(Action<string, string, string, JToken> action, string path, MustacheOptions options)
        {
            var outputPath = options.OutputPath.AddPathSeparator();
            var sourcePath = options.TemplatePath.AddPathSeparator();
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            foreach (var item in entities)
            {
                action.Invoke(path, outputPath, sourcePath, item);
            }

        }

        private void CreatePath(string path, string outputPath, string sourcePath, JToken item)
        {
            var destPath = pathRenderizer.TemplateFileName(path.Replace(sourcePath, outputPath, StringComparison.CurrentCultureIgnoreCase), item);
            if (destPath != null && !Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
        }

        private void TransformFile(string file, string outputPath, string sourcePath, JToken item)
        {
            var outpuFile = pathRenderizer.TemplateFileName(file.Replace(sourcePath, outputPath, StringComparison.CurrentCultureIgnoreCase), item);
            if (outpuFile != null)
            {
                var text = renderizer.Render(File.ReadAllText(file), item);
                File.WriteAllText(outpuFile, text);
                Console.WriteLine(outpuFile);
            }
        }
    }
}
