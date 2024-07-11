using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Exceptions;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace L2Data2Code.BaseGenerator.Services
{
    /// <summary>
    /// Code generator service
    /// </summary>
    public partial class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly ILogger logger;
        private readonly IMustacheRenderizer mustacheRenderizer;
        private readonly IConditionalPathRenderizer pathRenderizer;
        private readonly IMultiPathRenderizer multiPathRenderizer;
        private readonly IFileService fileService;
        private readonly ISchemaService schemaService;
        private readonly ITemplateService templateService;
        private readonly ISchemaFactory schemaFactory;
        private readonly IReplacementCollectionFactory replacementCollectionFactory;

        private readonly Dictionary<string, string> templateFiles;
        private readonly HashSet<string> referencedTables;
        private readonly Dictionary<string, object> internalVars;

        private static readonly CommentLine commentCsStyle = new() { Start = "// " };
        private static readonly CommentLine commentXmlStyle = new() { Start = "<!-- ", End = " -->" };
        private static readonly CommentLine commentCmdStyle = new() { Start = "# " };
        private static readonly CommentLine commentMdStyle = new() { Start = "[//]: # (", End = ")" };

        private static readonly Dictionary<string, CommentLine> commentByExtension = new()
        {
            { ".cs", commentCsStyle },
            { ".html", commentXmlStyle },
            { ".htm", commentXmlStyle },
            { ".xml", commentXmlStyle },
            { ".csproj", commentXmlStyle },
            { ".config", commentXmlStyle },
            { ".sln", commentCmdStyle },
            { ".md", commentMdStyle },
            { ".resx", commentXmlStyle },
        };

        private static readonly Dictionary<string, Regex> markByExtension = new()
        {
            { ".cs", CsMarkPartRegex() },
            { ".html", XmlMarkPartRegex() },
            { ".htm", XmlMarkPartRegex() },
            { ".xml", XmlMarkPartRegex() },
            { ".csproj", XmlMarkPartRegex() },
            { ".config", XmlMarkPartRegex() },
            { ".sln", CmdMarkPartRegex() },
            { ".md", MdMarkPartRegex() },
            { ".resx", XmlMarkPartRegex() },
        };


        private TemplateLibrary Library { get; set; }
        private Template Template { get; set; }
        private CodeGeneratorDto Options { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mustacheRenderizer">Mustache Renderizer service</param>
        /// <param name="schemaService">Schema service</param>
        /// <param name="logger">Logger service</param>
        /// <param name="templateService">Template service</param>
        /// <param name="schemaFactory">Scheme factory</param>
        /// <param name="pathRenderizer">Path renderizer</param>
        /// <param name="fileService">File service</param>
        /// <param name="replacementCollectionFactory">Replacement collection factory</param>
        /// <param name="multiPathRenderizer">Multi file renderizer</param>
        public CodeGeneratorService(IMustacheRenderizer mustacheRenderizer,
                                    ISchemaService schemaService,
                                    ILogger logger,
                                    ITemplateService templateService,
                                    ISchemaFactory schemaFactory,
                                    IConditionalPathRenderizer pathRenderizer,
                                    IFileService fileService,
                                    IReplacementCollectionFactory replacementCollectionFactory,
                                    IMultiPathRenderizer multiPathRenderizer)
        {
            this.mustacheRenderizer = mustacheRenderizer ?? throw new ArgumentNullException(nameof(mustacheRenderizer));
            this.schemaService = schemaService ?? throw new ArgumentNullException(nameof(schemaService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
            this.schemaFactory = schemaFactory ?? throw new ArgumentNullException(nameof(schemaFactory));
            this.pathRenderizer = pathRenderizer ?? throw new ArgumentNullException(nameof(pathRenderizer));
            this.fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            this.replacementCollectionFactory = replacementCollectionFactory ?? throw new ArgumentNullException(nameof(replacementCollectionFactory));
            this.multiPathRenderizer = multiPathRenderizer;

            referencedTables = new();
            templateFiles = new();
            internalVars = new();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">Options for generator</param>
        /// <param name="library">Template library, that is the content for "Templates.xml" file.</param>
        /// <param name="vars">Current collection of vars</param>
        public void Initialize(CodeGeneratorDto options, TemplateLibrary library, Dictionary<string, object> vars = null)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Library = library ?? throw new ArgumentNullException(nameof(library));
            referencedTables.Clear();
            templateFiles.Clear();

            try
            {
                Template = Library.GetTemplate(options.TemplateResource, options.Template);
                if (Template == null)
                {
                    throw new CodeGeneratorException($"Template resource not found {options.TemplateResource}", CodeGeneratorExceptionType.TemplateNotFound);
                }
                Options = options;
                if (!string.IsNullOrWhiteSpace(options.OutputPath))
                {
                    Template.SavePath = options.OutputPath;
                }
                Template.SavePath = Template.SavePath.AddPathSeparator();

                if (!string.IsNullOrWhiteSpace(options.Module))
                {
                    Template.Module = options.Module;
                }

                if (!string.IsNullOrWhiteSpace(options.Area))
                {
                    Template.Area = options.Area;
                }

                if (!string.IsNullOrWhiteSpace(options.Company))
                {
                    Template.Company = options.Company;
                }

                if (vars != null)
                {
                    internalVars.ClearAndAddRange(vars);
                }
                else
                {
                    CreateVarsFromUserVariables();
                }

                fileService.Initialize(options.Encoding, options.EndOfLine);
            }
            catch (CodeGeneratorException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CodeGeneratorException("Error loading template library: " + Path.Combine(options.TemplatePath, "Templates.xml"), ex, CodeGeneratorExceptionType.ErrorLoadingTemplate);
            }
        }

        /// <summary>
        /// Process tables and call <paramref name="onTableProcessed"/> when a table is processed. It can use <paramref name="alternativeDictionary"/> to set column descriptions
        /// </summary>
        /// <param name="onTableProcessed">Action for every table processed</param>
        /// <param name="alternativeDictionary">Alternative column descriptions</param>
        public void ProcessTables(Action<string> onTableProcessed = null, Dictionary<string, string> alternativeDictionary = null)
        {
            try
            {
                var tables = schemaService.Read(Options, alternativeDictionary);

                var selectedTables = Options.TableList.Select(s => s.ToUpper()).ToHashSet();
                var processTables = tables.Values
                    .Where(t => selectedTables.Contains(t.Name.ToUpper()))
                    .OrderBy(t => t.Name);

                if (!processTables.Any())
                {
                    logger.Warn($"Processing tables: There's nothing to process");
                    return;
                }

                if (Options.GenerateReferenced)
                {
                    referencedTables.Clear();
                    SetReferencedTables(processTables);
                    processTables = tables.Values
                        .Where(t => selectedTables.Contains(t.Name.ToUpper()) || referencedTables.Contains(t.Name.ToUpper()))
                        .OrderBy(t => t.Name);
                }

                var lastTable = processTables.Last();
                foreach (var table in processTables)
                {
                    logger.Info($"Processing table {table.Name}");
                    EntityTable tabla = new(table);
                    if (!Options.GeneateOnlyJson)
                    {
                        var results = GenerarCodigos(tabla);
                        SaveFiles(results, table == lastTable);
                    }
                    logger.Info($"Table {table.Name} has been processed");
                    onTableProcessed?.Invoke(table.Name);
                }

                if (Options.GenerateJsonInfo && Options.JsonGeneratedPath.NotEmpty() && Options.LastPass)
                {
                    var fileName = $"{Options.JsonGeneratedPath.AddPathSeparator()}{Options.SchemaName.ToSlug()}-dbinfo.json";
                    schemaService.GenerateJsonInfo(processTables, fileName);
                }
            }
            catch (CodeGeneratorException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.Error($"Processing files: {ex.Message}");
                throw;
            }
        }

        public string GetSolutionType() => Template.SolutionType;
        public Dictionary<string, object> GetVars() => internalVars;

        private ReplacementResult[] GenerarCodigos(EntityTable tabla)
        {
            StringExtensions.CurrentLang = schemaService.GetLang(Options.CreatedFromSchemaName);

            var replacement = replacementCollectionFactory.Create(tabla, Options, Template, internalVars);

            var templatesPath = templateService.GetPath(Template);

            if (templateFiles.Count == 0)
            {
                LoadTemplateFiles();
            }

            var outputBaseDir = mustacheRenderizer.RenderPath(Template.SavePath, replacement).AddPathSeparator();
            if (string.IsNullOrEmpty(outputBaseDir))
            {
                outputBaseDir = CodeGeneratorDto.DefaultOutputPath;
            }

            var replacementList = templateFiles.Keys
                .Where(f => !multiPathRenderizer.CanApplyMultiPath(f))
                .Select(templateFile =>
                {
                    ReplacementResult replacementResult;
                    var partToReplace = string.Empty;
                    string filePath, rawContent, fileExtension;

                    if (pathRenderizer.TryGetFileName(templatesPath, templateFile, replacement, out var filename))
                    {
                        var match = TemplateFilePartRegex().Match(filename);
                        if (match.Success)
                        {
                            filename = TemplateFilePartRegex().Replace(filename, "${start}${name}");
                            partToReplace = match.Groups["part"].Value;
                        }

                        filePath = Path.Combine(outputBaseDir, filename);
                        fileExtension = Path.GetExtension(filename.Replace(".template", string.Empty));
                        var commentLine = GetCommentLine(fileExtension);

                        var isBinaryFile = Path.GetFileName(templateFile).StartsWith('!');
                        rawContent = templateFiles[templateFile];
                        replacementResult = new(
                            Path.GetFileName(filePath),
                            filePath,
                            filePath.Replace(outputBaseDir, ""),
                            isBinaryFile ? () => rawContent : () => DoReplacement(replacement, partToReplace, filename, filePath, rawContent, commentLine));
                    }
                    else
                    {
                        replacementResult = null;
                    }

                    return replacementResult;
                })
            .Where(result => result != null)
            .ToList();

            var multiFileReplacement = templateFiles.Keys
                .Where(multiPathRenderizer.CanApplyMultiPath)
                .SelectMany(templateFile =>
                {
                    List<ReplacementResult> result = [];
                    var files = multiPathRenderizer.ApplyMultiPath(templateFile, templateFiles[templateFile], replacement);

                    foreach (var filename in files.Keys)
                    {
                        var filePath = Path.Combine(outputBaseDir, filename.Replace(templatesPath, ""));
                        var fileExtension = Path.GetExtension(filename.Replace(".template", string.Empty));
                        var commentLine = GetCommentLine(fileExtension);
                        var isBinaryFile = Path.GetFileName(templateFile).StartsWith('!');
                        var rawContent = files[filename];

                        result.Add(new ReplacementResult(
                            Path.GetFileName(filePath),
                            filePath,
                            filePath.Replace(outputBaseDir, ""),
                            () => rawContent));
                    }

                    return result;
                })
                .ToList();

            replacementList.AddRange(multiFileReplacement);

            return [.. replacementList];
        }

        private void LoadTemplateFiles()
        {
            try
            {
                templateFiles.Clear();

                var templatesPath = templateService.GetPath(Template);
                PreparePartials(templatesPath);

                var listOfTemplates = Directory.GetFiles(templatesPath, "*.*", SearchOption.AllDirectories);
                foreach (var templateFile in listOfTemplates)
                {
                    bool isBinaryFile = Path.GetFileName(templateFile).StartsWith('!');
                    var templateContent = isBinaryFile ? templateFile : fileService.ReadWithIncludes(templateFile, templatesPath);
                    templateFiles.Add(templateFile, templateContent);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error loading templates: {ex.Message}");
                throw;
            }
        }

        private void PreparePartials(string templatesPath)
        {
            var partialsFiles = fileService.GetPartials(templatesPath, Template.Partials);
            mustacheRenderizer.SetupPartials(partialsFiles);
        }

        private void SetReferencedTables(IEnumerable<Table> selectedTables)
        {
            if (!selectedTables.Any())
            {
                return;
            }

            List<Table> tableList = new();
            foreach (var item in selectedTables)
            {
                var tableName = item.Name.ToUpper();
                if (!referencedTables.Contains(tableName))
                {
                    var fks = item.OuterKeys.Select(r => r.ColumnReferenced.Table);
                    var tablesToAdd = fks.Where(t => !tableList.Any(l => l.Name.Equals(t.Name)));
                    tableList.AddRange(tablesToAdd);

                    var collections = item.InnerKeys.Select(r => r.ColumnReferencing.Table);
                    var tablesCollectionsToAdd = collections.Where(t => !tableList.Any(l => l.Name.Equals(t.Name)));
                    tableList.AddRange(tablesCollectionsToAdd);
                }
                referencedTables.Add(tableName);

            }

            SetReferencedTables(tableList);

        }

        private void SaveFiles(ReplacementResult[] results, bool lastToProcess = false)
        {
            List<ReplacementResult> lastResults = new();
            foreach (var result in results)
            {
                var fileName = Path.GetFileName(result.FileName).Replace(".template", string.Empty);
                var path = Path.GetDirectoryName(result.FileName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (!result.IsBinaryFile)
                {
                    var content = result.Content;
                    if (lastToProcess && Options.LastPass && content.Contains("!!!ENDOF"))
                    {
                        lastResults.Add(result);
                    }
                    fileService.Write(Path.Combine(path, fileName), content);
                }
                else
                {
                    fileService.Copy(result.Content, Path.Combine(path, fileName[1..]));
                }
            }

            foreach (var result in lastResults)
            {
                var fileExtension = Path.GetExtension(result.FileName.Replace(".template", string.Empty));
                var content = GetMarkRegex(fileExtension).Replace(result.Content, string.Empty);
                fileService.Write(result.FileName.Replace(".template", string.Empty), content);
            }
        }

        private string DoReplacement(Dictionary<string, object> replacement, string partToReplace, string filename, string filePath, string rawContent, CommentLine commentLine)
        {
            string content;

            try
            {
                content = mustacheRenderizer.Render(rawContent, replacement);
            }
            catch (Exception ex)
            {
                throw new CodeGeneratorException($"Error rendering {filename}", ex);
            }

            if (!string.IsNullOrEmpty(partToReplace))
            {
                string prevContent;
                if (File.Exists(filePath))
                {
                    prevContent = fileService.Read(filePath);
                }
                else
                {
                    prevContent = $"{commentLine.Start}!!!ENDOF{partToReplace}{commentLine.End}\n";
                }

                if (prevContent.IndexOf(content) < 0)
                {
                    content = prevContent.Replace($"{commentLine.Start}!!!ENDOF{partToReplace}{commentLine.End}", $"{content}{commentLine.Start}!!!ENDOF{partToReplace}{commentLine.End}");
                }
                else
                {
                    content = prevContent;
                }
            }

            if (Options.CleanEndOfCodeLine)
            {
                content = TemplateCleanRegex().Replace(content, ";");
            }

            return content.RemoveDuplicates();
        }

        private static CommentLine GetCommentLine(string fileExtension) =>
            commentByExtension.TryGetValue(fileExtension, out var value) ? value : commentCsStyle;

        private static Regex GetMarkRegex(string fileExtension) =>
            markByExtension.TryGetValue(fileExtension, out var value) ? value : CsMarkPartRegex();

        private void CreateVarsFromUserVariables()
        {
            var variables = string.Concat(Library.Global?.Vars ?? string.Empty, ";",
                                          Template.UserVariables ?? string.Empty, ";",
                                          Options.UserVariables ?? string.Empty, ";",
                                          Template.FinalVariables ?? string.Empty, ";",
                                          Library.Global?.FinalVars ?? string.Empty, ";",
                                          $"{nameof(Template.SavePath)}={Template.SavePath};");

            var allVars = variables.ReplaceEndOfLine(";").Split(';', StringSplitOptions.RemoveEmptyEntries);

            internalVars.Clear();

            internalVars.Add("database", Options.SchemasConfiguration[Options.CreatedFromSchemaName].OverrideDataBaseId ?? schemaFactory.GetProviderDefinitionKey(Options.CreatedFromSchemaName));
            internalVars.Add(nameof(Template), Template.Name);
            internalVars.Add(nameof(Template.Company), Template.Company);
            internalVars.Add(nameof(Template.Area), Template.Area);
            internalVars.Add(nameof(Template.Module), Template.Module);
            internalVars.Add(nameof(Options.GeneratorApplication), Options.GeneratorApplication);
            internalVars.Add(nameof(Options.GeneratorVersion), Options.GeneratorVersion);
            internalVars.Add(nameof(Options.MinorVersion), Options.MinorVersion);
            internalVars.Add(nameof(Options.MajorVersion), Options.MajorVersion);
            internalVars.Add(nameof(Options.TemplatePath), Options.TemplatePath.AddPathSeparator());

            ProcessConditionals(allVars);
        }

        private void ProcessConditionals(string[] allVars)
        {
            var lastConditionResult = false;
            string value;
            foreach (var item in allVars.Select(s => s.Trim()))
            {
                var itemLine = item;
                if (string.IsNullOrWhiteSpace(item) || !item.Contains('='))
                {
                    continue;
                }

                var matchCondition = TemplateConditionRegex().Match(item);
                if (matchCondition.Success)
                {
                    var key = matchCondition.Groups["key"].Value;
                    value = matchCondition.Groups["value"].Value;
                    if (internalVars.TryGetValue(key, out var internalVar) && internalVar.ToString().ToLower() == value?.ToLower())
                    {
                        lastConditionResult = true;
                        itemLine = "." + matchCondition.Groups["var"].Value;
                    }
                    else
                    {
                        lastConditionResult = false;
                        continue;
                    }
                }
                if (itemLine.StartsWith("."))
                {
                    if (lastConditionResult)
                    {
                        itemLine = itemLine[1..];
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    lastConditionResult = false;
                }
                var camps = itemLine.Split('=');
                value = camps[1].Trim();
                value = value.NotEmpty() && value.Contains("{{")
                    ? value.Contains('\\') ? mustacheRenderizer.RenderPath(value, internalVars) : mustacheRenderizer.Render(value, internalVars)
                    : value;

                if (value.Equals("true", StringComparison.CurrentCultureIgnoreCase) || value.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                {
                    internalVars[camps[0].Trim()] = bool.Parse(value);
                }
                else
                {
                    internalVars[camps[0].Trim()] = value;
                }
            }
        }

        [GeneratedRegex("(?<start>^|\\\\)addtofile-(?<part>[0-9]+)-(?<name>.+)$", RegexOptions.Compiled | RegexOptions.Singleline)]
        private static partial Regex TemplateFilePartRegex();
        [GeneratedRegex("^\\s*if\\s+(?<key>[^=]+)=(?<value>[^\\s]+)\\s+(?<var>.+)$", RegexOptions.Compiled | RegexOptions.Singleline)]
        private static partial Regex TemplateConditionRegex();
        [GeneratedRegex("(\\s|\\n)+;", RegexOptions.Multiline | RegexOptions.Compiled)]
        private static partial Regex TemplateCleanRegex();
        [GeneratedRegex("// !!!ENDOF(?<part>[0-9]+)", RegexOptions.Compiled | RegexOptions.Singleline)]
        private static partial Regex CsMarkPartRegex();
        [GeneratedRegex("<!-- !!!ENDOF(?<part>[0-9]+) -->", RegexOptions.Compiled | RegexOptions.Singleline)]
        private static partial Regex XmlMarkPartRegex();
        [GeneratedRegex("# !!!ENDOF(?<part>[0-9]+)", RegexOptions.Compiled | RegexOptions.Singleline)]
        private static partial Regex CmdMarkPartRegex();
        [GeneratedRegex("\\[//\\]: # \\(!!!ENDOF(?<part>[0-9]+)\\)", RegexOptions.Compiled | RegexOptions.Singleline)]
        private static partial Regex MdMarkPartRegex();
    }
}
