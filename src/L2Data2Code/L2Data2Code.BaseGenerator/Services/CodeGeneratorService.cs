using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Exceptions;
using L2Data2Code.BaseGenerator.Extensions;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace L2Data2Code.BaseGenerator.Services
{
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly ILogger logger;
        private readonly IMustacheRenderizer mustacheRenderizer;
        private readonly ISchemaService schemaService;

        private readonly Dictionary<string, string> templateFiles;
        private readonly HashSet<string> referencedTables;
        private readonly Dictionary<string, object> internalVars;

        private const string INCLUDE_TEXT = "{{!include ";

        private static readonly Regex templateFilePart = new(@"(?<start>^|\\)addtofile-(?<part>[0-9]+)-(?<name>.+)$", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex templateCondition = new(@"^\s*if\s+(?<key>[^=]+)=(?<value>[^\s]+)\s+(?<var>.+)$", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex templateClean = new(@"(\s|\n)+;", RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex csMarkPart = new(@"// !!!ENDOF(?<part>[0-9]+)", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex xmlMarkPart = new(@"<!-- !!!ENDOF(?<part>[0-9]+) -->", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex cmdMarkPart = new(@"# !!!ENDOF(?<part>[0-9]+)", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex mdMarkPart = new(@"\[//\]: # \(!!!ENDOF(?<part>[0-9]+)\)", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex includeRegex = new(@"\{\{!include\s+(?<name>[^\}]+)\}\}", RegexOptions.Singleline | RegexOptions.Compiled);

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
        };

        private static readonly Dictionary<string, Regex> markByExtension = new()
        {
            { ".cs", csMarkPart },
            { ".html", xmlMarkPart },
            { ".htm", xmlMarkPart },
            { ".xml", xmlMarkPart },
            { ".csproj", xmlMarkPart },
            { ".config", xmlMarkPart },
            { ".sln", cmdMarkPart },
            { ".md", mdMarkPart },
        };


        private TemplateLibrary Library { get; set; }
        private Template Template { get; set; }
        private CodeGeneratorDto Options { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mustacheRenderizer">Mustache Renderizer service</param>
        /// <param name="schemaService">Schema service</param>
        public CodeGeneratorService(IMustacheRenderizer mustacheRenderizer, ISchemaService schemaService, ILogger logger)
        {
            this.mustacheRenderizer = mustacheRenderizer ?? throw new ArgumentNullException(nameof(mustacheRenderizer));
            this.schemaService = schemaService ?? throw new ArgumentNullException(nameof(schemaService));
            this.logger = logger;
            referencedTables = new();
            templateFiles = new();
            internalVars = new();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mustacheRenderizer">Mustache Renderizer service</param>
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
                Template = Library.GetTemplate(options.TemplateResource);
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

                FileService.SetSettings(options.Encoding, options.EndOfLine);
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

        public void ProcessTables(Action<string> onTableProcessed = null, Dictionary<string, string> alternativeDictionary = null)
        {
            try
            {
                var tables = schemaService.Read(Options, alternativeDictionary);

                HashSet<string> selectedTables = Options.TableList.Select(s => s.ToUpper()).ToHashSet();
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
                    GenerateJsonInfo(processTables);
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
            mustacheRenderizer.SetIsoLanguaje(Config.GetLang(Options.CreatedFromSchemaName));

            var replacement = GetReplacementData(tabla);

            var templatesPath = Template.GetPath();

            if (!templateFiles.Any())
            {
                LoadTemplateFiles();
            }

            var outputBaseDir = mustacheRenderizer.Render(Template.SavePath, replacement).AddPathSeparator();
            if (string.IsNullOrEmpty(outputBaseDir))
                outputBaseDir = "C:\\src\\tmp\\";

            return templateFiles.Keys
                .Select(templateFile =>
                {
                    var partToReplace = string.Empty;
                    string filename, filePath, rawContent, fileExtension;

                    filename = TemplateFileName(templatesPath, templateFile, replacement);
                    if (filename == null)
                    {
                        return null;
                    }
                    //Check to add to a file
                    var match = templateFilePart.Match(filename);
                    if (match.Success)
                    {
                        filename = templateFilePart.Replace(filename, "${start}${name}");
                        partToReplace = match.Groups["part"].Value;
                    }

                    filePath = Path.Combine(outputBaseDir, filename);
                    fileExtension = Path.GetExtension(filename.Replace(".template", string.Empty));
                    var commentLine = GetCommentLine(fileExtension);

                    rawContent = templateFiles[templateFile];
                    ReplacementResult replacementResult = new(Path.GetFileName(filePath),
                                                                filePath,
                                                                filePath.Replace(outputBaseDir, ""),
                                                                () => DoReplacement(replacement, partToReplace, filename, filePath, rawContent, commentLine));

                    return replacementResult;
                })
            .Where(result => result != null)
            .ToArray();
        }

        private void LoadTemplateFiles()
        {
            try
            {
                templateFiles.Clear();

                var templatesPath = Template.GetPath();

                var listOfTemplates = Directory.GetFiles(templatesPath, "*.*", SearchOption.AllDirectories);
                foreach (var templateFile in listOfTemplates)
                {
                    var templateContent = FileService.Read(templateFile);
                    if (templateContent.Contains(INCLUDE_TEXT))
                    {
                        var lines = templateContent.Split('\n');
                        List<string> replacedLines = new();
                        var changed = false;
                        foreach (var line in lines)
                        {
                            if (line.StartsWith(INCLUDE_TEXT))
                            {
                                var match = includeRegex.Match(line);
                                if (match.Success)
                                {
                                    var file = match.Groups["name"].Value;
                                    var fileContent = FileService.Read(Path.Combine(templatesPath, file));
                                    replacedLines.Add(fileContent.ReplaceEndOfLine("\n"));
                                    changed = true;
                                    continue;
                                }

                            }
                            replacedLines.Add(line);
                        }

                        if (changed)
                        {
                            templateContent = string.Join("\n", replacedLines);
                        }

                    }
                    templateFiles.Add(templateFile, templateContent);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error loading templates: {ex.Message}");
                throw;
            }

        }

        private void GenerateJsonInfo(IEnumerable<Table> processTables)
        {
            // Para generar un json con cada una de las tablas
            PropertyRenameAndIgnoreSerializerContractResolver jsonResolver = new();
            jsonResolver.IgnoreProperty(typeof(Column),
                nameof(Column.Table),
                nameof(Column.FullName),
                nameof(Column.FullNameWithOwner),
                nameof(Column.PropertyName)
                );
            jsonResolver.IgnoreProperty(typeof(Table),
                nameof(Table.PK),
                nameof(Table.CleanName),
                nameof(Table.ClassName)
                );
            jsonResolver.IgnoreProperty(typeof(Key),
                nameof(Key.ColumnReferenced),
                nameof(Key.ColumnReferencing)
                );

            var fileName = $"{Options.JsonGeneratedPath.AddPathSeparator()}{Options.SchemaName.ToSlug()}-dbinfo.json";

            FileService.Write(fileName, JsonConvert.SerializeObject(processTables, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = jsonResolver,
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                }));
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
                var path = Path.GetDirectoryName(result.FileName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var content = result.Content;
                if (lastToProcess && Options.LastPass && content.Contains("!!!ENDOF"))
                {
                    lastResults.Add(result);
                }
                FileService.Write(result.FileName.Replace(".template", string.Empty), content);
            }

            foreach (var result in lastResults)
            {
                var fileExtension = Path.GetExtension(result.FileName.Replace(".template", string.Empty));
                var content = GetMarkRegex(fileExtension).Replace(result.Content, string.Empty);
                FileService.Write(result.FileName.Replace(".template", string.Empty), content);
            }
        }

        private string DoReplacement(Replacement replacement, string partToReplace, string filename, string filePath, string rawContent, CommentLine commentLine)
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
                    prevContent = FileService.Read(filePath);
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
                content = templateClean.Replace(content, ";");
            }

            return content;
        }

        private static CommentLine GetCommentLine(string fileExtension) =>
            commentByExtension.ContainsKey(fileExtension) ? commentByExtension[fileExtension] : commentCsStyle;

        private static Regex GetMarkRegex(string fileExtension) =>
            markByExtension.ContainsKey(fileExtension) ? markByExtension[fileExtension] : csMarkPart;

        private string TemplateFileName(string templatesPath, string filePath, Replacement replacement)
        {
            var partialName = filePath.Replace(templatesPath, "");

            // Moustached dots are part of file/folder name
            partialName = partialName.Replace("{{.}}", ".");

            //Check conditional template
            var iwhen = partialName.IndexOf("when{{");
            while (iwhen >= 0)
            {
                var itag = iwhen + "when{{".Length;
                var ewhen = partialName.IndexOf("}}", itag);
                var tag = partialName[itag..ewhen];
                if (!CheckTemplateName(tag, replacement))
                    return null;
                partialName = partialName[..iwhen] + partialName[(ewhen + "}}".Length)..];
                iwhen = partialName.IndexOf("when{{");
            }

            // Apply template replacement
            partialName = mustacheRenderizer.Render(partialName, replacement);

            return partialName;
        }

        private static bool CheckTemplateName(string tag, Replacement replacement)
        {
            return tag.Contains('=') ? CheckTemplateNameConditionIsTrue(tag, replacement) : CheckTemplateNameIsTrue(tag, replacement);
        }

        private static bool CheckTemplateNameIsTrue(string tag, Replacement replacement)
        {
            var result = false;

            var type = typeof(Replacement);
            var prop = type.GetProperty(tag.TrimStart('^'), typeof(bool));
            if (prop != null)
                result = (bool)prop.GetValue(replacement, null);

            if (tag.StartsWith("^"))
                return !result;

            return result;
        }

        private static bool CheckTemplateNameConditionIsTrue(string tag, Replacement replacement)
        {
            var result = false;

            var expression = tag.TrimStart('^').Split('=');
            var key = expression[0].Trim();
            var value = expression[1].Trim();
            if (replacement.ContainsKey(key) && replacement[key].ToString() == value)
            {
                result = true;
            }

            if (tag.StartsWith("^"))
                return !result;

            return result;
        }

        private void CreateVarsFromUserVariables()
        {
            var variables = $"{Library.Global?.Vars ?? string.Empty}";
            variables += $";{Template.UserVariables ?? string.Empty}";
            variables += $";{Options.UserVariables ?? string.Empty}";
            variables += $";{Template.FinalVariables ?? string.Empty}";
            variables += $";{Library.Global?.FinalVars ?? string.Empty}";

            var allVars = variables.ReplaceEndOfLine(";").Split(';', StringSplitOptions.RemoveEmptyEntries);

            internalVars.Clear();

            internalVars.Add("database", SchemaFactory.GetProviderDefinitionKey(Options.CreatedFromSchemaName));
            internalVars.Add(nameof(Template.Company), Template.Company);
            internalVars.Add(nameof(Template.Area), Template.Area);
            internalVars.Add(nameof(Template.Module), Template.Module);
            internalVars.Add(nameof(Options.GeneratorApplication), Options.GeneratorApplication);
            internalVars.Add(nameof(Options.GeneratorVersion), Options.GeneratorVersion);
            internalVars.Add(nameof(Template.SavePath), mustacheRenderizer.Render(Template.SavePath, internalVars));

            var lastConditionResult = false;
            string value;
            foreach (var item in allVars.Select(s => s.Trim()))
            {
                var itemLine = item;
                if (string.IsNullOrWhiteSpace(item) || !item.Contains('=')) continue;
                var matchCondition = templateCondition.Match(item);
                if (matchCondition.Success)
                {
                    var key = matchCondition.Groups["key"].Value;
                    value = matchCondition.Groups["value"].Value;
                    if (internalVars.ContainsKey(key) && internalVars[key].ToString() == value)
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
                        itemLine = itemLine[1..];
                    else
                        continue;
                }
                else
                {
                    lastConditionResult = false;
                }
                var camps = itemLine.Split('=');
                value = camps[1].Trim();
                internalVars[camps[0].Trim()] = value.NotEmpty() && value.Contains("{{")
                    ? mustacheRenderizer.Render(value, internalVars)
                    : value;
            }
        }

        private Replacement GetReplacementData(EntityTable table)
        {
            var (ConnectionString, Provider) = Config.GetConnectionString(Options.CreatedFromSchemaName);

            var tableName = table.TableName;
            var normalizeNames = Config.NormalizedNames(Options.CreatedFromSchemaName);

            var properties =
                table.Columns.Select(
                    (column, index, isFirst, isLast) =>
                    {
                        var name = column.Name;
                        var type = DecodeCSharpType(column.Type);
                        Property property = new()
                        {
                            Table = tableName,
                            Name = name,
                            Nullable = column.IsNull,
                            PrimaryKey = column.PrimaryKey,
                            IsFirst = isFirst,
                            IsLast = isLast,
                            DefaultValue = column.GetDefaultValue(),
                            Type = type,
                            OverrideDbType = SchemaFactory.GetConversion(Provider, type),
                            Description = string.IsNullOrWhiteSpace(column.Description) ? null : column.Description.ReplaceEndOfLine(),
                            IsCollection = column.IsCollection,
                            IsForeignKey = column.IsForeignKey,
                            ColumnName = column.ColumnName,
                            ColumnNameOrName = normalizeNames ? name : column.ColumnName,
                            IsAutoIncrement = column.IsAutoIncrement,
                            IsComputed = column.IsComputed,
                            PkOrder = column.PkOrder,
                            MultiplePKColumns = table.MultiplePKColumns,
                            Precision = column.Precision,
                            Scale = column.Scale,
                            IsNumeric = column.IsNumeric,
                            IsString = column.Type.StartsWith("string"),
                            IsDateOrTime = column.Type.StartsWith("DateTime") || column.Type.StartsWith("TimeSpan"),
                            Join = column.Join,
                            FromField = column.FromField,
                            ToField = column.ToField,
                            DbJoin = column.DbJoin,
                            DbFromField = column.DbFromField,
                            DbToField = column.DbToField,
                        };
                        return property;
                    }).ToArray();

            Entity entity = new()
            {
                Name = table.ClassName,
                UseSpanish = Config.GetLang(Options.CreatedFromSchemaName).Equals("es", StringComparison.CurrentCultureIgnoreCase),
                MultiplePKColumns = table.MultiplePKColumns,
            };

            Replacement currentReplacement = new()
            {
                Entity = entity,
                IsView = table.IsView,
                IsUpdatable = table.IsUpdatable,
                Description = string.IsNullOrWhiteSpace(table.Description) ? null : table.Description.ReplaceEndOfLine(),
                ConnectionString = ConnectionString,
                DataProvider = Provider,
                Module = Template.Module,
                Area = Template.Area,
                Company = Template.Company,
                TableName = tableName,
                TableNameOrEntity = normalizeNames ? entity.Name : tableName,
                GenerateReferences = Options.GenerateReferenced,
                IgnoreColumns = Template.IgnoreColumns == null ? Array.Empty<string>() : Template.IgnoreColumns.Replace(" ", "").Split(Path.PathSeparator),
                UnfilteredColumns = properties,
                GenerateBase = false,
                Vars = internalVars,
                CanCreateDB = Config.CanCreateDB(Options.CreatedFromSchemaName),
            };

            return currentReplacement;
        }

        private static string DecodeCSharpType(string type) =>
            type.StartsWith(Constants.InternalTypes.Collection) || type.StartsWith(Constants.InternalTypes.ReferenceTo) ? type[1..] : type;
    }
}
