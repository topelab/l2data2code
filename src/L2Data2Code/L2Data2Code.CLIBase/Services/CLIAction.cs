using L2Data2Code.CLIBase.Interfaces;
using L2Data2Code.CLIBase.Options;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace L2Data2Code.CLIBase.Services
{
    /// <summary>
    /// Run a HandleBars action
    /// </summary>
    public class CLIAction : ICLIAction
    {
        private readonly IMustacheRenderizer renderizer;
        private readonly IFileExecutor fileExecutor;
        private readonly ICLIOptionsInitializer CLIOptionsInitializer;
        private readonly IConditionalPathRenderizer conditionalPathRenderizer;
        private readonly IFileService fileService;
        private readonly IMultiPathRenderizer multiPathRenderizer;
        private ICLIOptions options;
        private JToken entities;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="renderizer">Renderer</param>
        /// <param name="fileExecutor">File executor</param>
        /// <param name="CLIOptionsInitializer">HandleBars options initializer</param>
        /// <param name="conditionalPathRenderizer">Path renderer</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CLIAction(IMustacheRenderizer renderizer, IFileExecutor fileExecutor, ICLIOptionsInitializer CLIOptionsInitializer, IConditionalPathRenderizer conditionalPathRenderizer, IFileService fileService, IMultiPathRenderizer multiPathRenderizer)
        {
            this.renderizer = renderizer ?? throw new ArgumentNullException(nameof(renderizer));
            this.fileExecutor = fileExecutor ?? throw new ArgumentNullException(nameof(fileExecutor));
            this.CLIOptionsInitializer = CLIOptionsInitializer ?? throw new ArgumentNullException(nameof(CLIOptionsInitializer));
            this.conditionalPathRenderizer = conditionalPathRenderizer ?? throw new ArgumentNullException(nameof(conditionalPathRenderizer));
            this.fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            this.multiPathRenderizer = multiPathRenderizer ?? throw new ArgumentNullException(nameof(multiPathRenderizer));
        }

        /// <summary>
        /// Initialize options
        /// </summary>
        /// <param name="options">HandleBars options</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Initialize(ICLIOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            fileExecutor.Initialize(options.TemplatePath);
            entities = CLIOptionsInitializer.Initialize(options);
            PreparePartials(options.TemplatePath);
        }

        /// <summary>
        /// Run HandleBars action
        /// </summary>
        public void Run()
        {
            if (entities != null)
            {
                fileExecutor.RunOnPaths((path) => DoAction(CreatePath, path, options));
                fileExecutor.RunOnFiles((file) => DoAction(TransformFile, file, options));
            }
        }

        private void PreparePartials(string templatesPath)
        {
            var partialsFiles = fileService.GetPartials(templatesPath);
            renderizer.SetupPartials(partialsFiles);
        }

        private void DoAction(Action<string, string, string, JToken> action, string pathOrFile, ICLIOptions options)
        {
            var outputPath = options.OutputPath.AddPathSeparator();
            var sourcePath = options.TemplatePath.AddPathSeparator();
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            foreach (var item in entities)
            {
                action.Invoke(pathOrFile, outputPath, sourcePath, item);
            }

        }

        private void CreatePath(string path, string outputPath, string sourcePath, JToken item)
        {
            var canRenderPath = conditionalPathRenderizer.TryGetFileName(path.Replace(sourcePath, outputPath, StringComparison.CurrentCultureIgnoreCase), item, out var destPath);
            if (canRenderPath && !Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
        }

        private void TransformFile(string file, string outputPath, string sourcePath, JToken item)
        {
            if (conditionalPathRenderizer.TryGetFileName(file.Replace(sourcePath, outputPath, StringComparison.CurrentCultureIgnoreCase), item, out var outpuFile))
            {
                var originalText = fileService.ReadWithIncludes(file, sourcePath);

                if (multiPathRenderizer.CanApplyMultiPath(file))
                {
                    var files = multiPathRenderizer.ApplyMultiPath(file.Replace(sourcePath, outputPath, StringComparison.CurrentCultureIgnoreCase), originalText, item);
                    foreach (var key in files.Keys)
                    {
                        fileService.Write(key, files[key]);
                    }
                }
                else
                {
                    var text = renderizer.Render(originalText, item);
                    fileService.Write(outpuFile, text);
                }
                Console.WriteLine(outpuFile);
            }
        }
    }
}
