using L2Data2Code.BaseGenerator.Configuration;
using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Exceptions;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.BaseMustache.Extensions;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using L2Data2CodeUI.Shared.Dto;
using L2Data2CodeUI.Shared.Localize;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace L2Data2CodeUI.Shared.Adapters
{
    public class GeneratorAdapter : IGeneratorAdapter
    {
        #region Private Fields

        private readonly string schemaNameToFake = "general";
        private readonly ILogger logger;
        private Dictionary<string, string> _alternativeDictionary = new();
        private string schemaName = "localserver";
        private string descriptionsSchemaName = "commentserver";
        private string outputSchemaName = "localserver";
        private ISchemaReader schemaReader;
        private IEnumerable<string> slnFiles;
        private readonly IAppService appService;
        private readonly IMessageService messageService;
        private readonly ICommandService commandService;
        private readonly IGitService gitService;
        private readonly ICodeGeneratorService codeGeneratorService;
        private readonly IFileMonitorService fileMonitorService;
        private readonly IJsonSetting jsonSetting;
        private Tables tables;
        private readonly StringBuilderWriter writer = new();
        private readonly ISchemaOptionsFactory schemaOptionsFactory;
        private readonly ISchemaService schemaService;
        private readonly ITemplateService templateService;
        private readonly ISchemaFactory schemaFactory;

        #endregion Private Fields

        #region Public Constructors

        public GeneratorAdapter(IMessageService messageService,
                                IAppService appService,
                                ICommandService commandService,
                                IGitService gitService,
                                ICodeGeneratorService codeGeneratorService,
                                IJsonSetting jsonSetting,
                                IFileMonitorService fileMonitorService,
                                ILogger logger,
                                IAppSettingsConfiguration settingsConfiguration,
                                IGlobalsConfiguration globalsConfiguration,
                                IDataSorcesConfiguration areasConfiguration,
                                IBasicConfiguration<ModuleConfiguration> modulesConfiguration,
                                IBasicConfiguration<SchemaConfiguration> schemasConfiguration,
                                ITemplatesConfiguration templatesConfiguration,
                                ISchemaOptionsFactory schemaOptionsFactory,
                                ISchemaService schemaService,
                                ITemplateService templateService,
                                ISchemaFactory schemaFactory)
        {
            this.messageService = messageService;
            OutputPath = CodeGeneratorDto.DefaultOutputPath;
            this.appService = appService;
            this.commandService = commandService;
            this.gitService = gitService;
            this.codeGeneratorService = codeGeneratorService;
            this.fileMonitorService = fileMonitorService;
            this.jsonSetting = jsonSetting;
            this.logger = logger;
            this.schemaOptionsFactory = schemaOptionsFactory;
            this.schemaService = schemaService;
            this.templateService = templateService;
            this.schemaFactory = schemaFactory ?? throw new ArgumentNullException(nameof(schemaFactory));

            SettingsConfiguration = settingsConfiguration;
            GlobalsConfiguration = globalsConfiguration;
            DataSourcesConfiguration = areasConfiguration;
            ModulesConfiguration = modulesConfiguration;
            SchemasConfiguration = schemasConfiguration;
            TemplatesConfiguration = templatesConfiguration;

            jsonSetting.AddSettingFiles(SettingsConfiguration["TemplateSettings"]);
            SettingsConfiguration.Merge(jsonSetting.Config[SectionLabels.APP_SETTINGS].ToNameValueCollection());
            SettingsConfiguration["TemplatesBasePath"] ??= Path.GetDirectoryName(SettingsConfiguration["TemplateSettings"]).AddPathSeparator();

            this.fileMonitorService.StartMonitoring(CheckTemplateFileChanges, SettingsConfiguration["TemplatesBasePath"], "*.json");
        }

        #endregion Public Constructors

        #region Public Properties

        public Action OnConfigurationChanged { get; set; }
        public IBasicNameValueConfiguration SettingsConfiguration { get; }
        public IDataSorcesConfiguration DataSourcesConfiguration { get; }
        public IBasicConfiguration<ModuleConfiguration> ModulesConfiguration { get; }
        public IBasicConfiguration<SchemaConfiguration> SchemasConfiguration { get; }
        public ITemplatesConfiguration TemplatesConfiguration { get; }
        public IGlobalsConfiguration GlobalsConfiguration { get; }
        public string OutputPath { get; set; }
        public string SelectedDataSource { get; private set; }
        public string SelectedModule { get; private set; }
        public string SelectedTemplate { get; private set; }
        public string SelectedVars { get; private set; }
        public string SlnFile => slnFiles?.FirstOrDefault() ?? $"{OutputPath?.TrimPathSeparator()}\\{SelectedModule}.sln".ToLower();
        public string SolutionType { get; set; }
        public Tables Tables { get => tables ?? new Tables(); }
        public Dictionary<string, object> CompiledVars { get; private set; } = new Dictionary<string, object>();
        public string GeneratorApplication { get; set; }
        public string GeneratorVersion { get; set; }
        public string InputSourceType { get; private set; }

        #endregion Public Properties

        #region Public Methods


        public IEnumerable<Table> GetAllTables() => Tables.Select(t => t.Value);

        public IEnumerable<string> GetAreaList()
            => DataSourcesConfiguration.GetKeys();

        public IEnumerable<string> GetModuleList(string selectedDataSource)
            => ModulesConfiguration.GetKeys().Where(s => s.StartsWith(DataSourcesConfiguration[selectedDataSource].Area + "."));

        public IEnumerable<string> GetTemplateList()
            => TemplatesConfiguration.GetKeys();

        public IEnumerable<string> GetVarsList(string selectedTemplate)
            => TemplatesConfiguration[selectedTemplate].Configurations?.AllKeys.AsEnumerable();

        public void Run(CodeGeneratorDto baseOptions)
        {
            messageService.Clear(MessageCodes.RUN_COMMAND);

            if (!Directory.Exists(baseOptions.OutputPath))
            {
                Directory.CreateDirectory(baseOptions.OutputPath);
            }
            gitService.GitInit(baseOptions.OutputPath);

            var basePath = SettingsConfiguration["TemplatesBasePath"].AddPathSeparator();
            var templatePath = TemplatesConfiguration[SelectedTemplate].Path;
            CodeGeneratorDto options = new()
            {
                Area = DataSourcesConfiguration[SelectedDataSource].Area,
                Module = ModulesConfiguration[SelectedModule].Name,
                GenerateReferenced = baseOptions.GenerateReferenced,
                RemoveFolders = baseOptions.RemoveFolders && !baseOptions.GeneateOnlyJson,
                OutputPath = baseOptions.OutputPath.AddPathSeparator(),
                CreatedFromSchemaName = outputSchemaName,
                UserVariables = TemplatesConfiguration[SelectedTemplate].Configurations?[SelectedVars],
                TemplatePath = Path.Combine(basePath, templatePath),
                TemplateResource = TemplatesConfiguration.Resource(SelectedTemplate),
                GeneratorApplication = baseOptions.GeneratorApplication,
                GeneratorVersion = baseOptions.GeneratorVersion,
                GeneateOnlyJson = baseOptions.GeneateOnlyJson,
                Encoding = SettingsConfiguration[nameof(CodeGeneratorDto.Encoding)] ?? "utf8",
                EndOfLine = SettingsConfiguration[nameof(CodeGeneratorDto.EndOfLine)] ?? Environment.NewLine,
                SchemasConfiguration = SchemasConfiguration,
            };

            logger.Info("Starting generation");


            if (!CheckConnection())
            {
                return;
            }

            try
            {
                logger.Info($"Options running: {JsonConvert.SerializeObject(options)}");
            }
            catch (Exception ex)
            {
                messageService.Error($"GeneratorAdapter.Run(...) - {Messages.ErrorSerializingOptions}: {ex.Message}", Messages.ErrorSerializingOptions, MessageCodes.RUN_GENERATOR);
                return;
            }

            if (baseOptions.RemoveFolders && !TemplatesConfiguration.HasToRemoveFolders(SelectedTemplate))
            {
                options.RemoveFolders = false;
                messageService.Info(Messages.ErrorCannotRemoveFolders, MessageCodes.RUN_GENERATOR);
            }

            var path = $"{options.OutputPath}";
            gitService.GitCommit(path);

            if (options.RemoveFolders)
            {
                try
                {
                    EmptyOutputPath(path);
                }
                catch (Exception ex)
                {
                    messageService.Error($"GeneratorAdapter.Run(...) - Removing folders : {ex.Message}", Messages.ErrorRemovingFolders, MessageCodes.RUN_GENERATOR);
                    gitService.GitReset(path);
                    return;
                }
            }

            try
            {
                HashSet<string> processedTemplates = new();
                var library = templateService.TryLoad(options.TemplatePath, options.TemplateResource);

                if (library == null)
                {
                    var msg = string.Format(Messages.ErrorResourceNotFound, options.TemplateResource, options.TemplatePath);
                    messageService.Error(msg, msg, MessageCodes.RUN_GENERATOR);
                    gitService.GitReset(path);
                    return;
                }

                var template = library.Templates.FirstOrDefault(t => t.ResourcesFolder.Equals(options.TemplateResource, StringComparison.CurrentCultureIgnoreCase));

                var isFirst = true;

                while (template != null)
                {
                    if (processedTemplates.Contains(template.ResourcesFolder))
                    {
                        break;
                    }
                    processedTemplates.Add(template.ResourcesFolder);

                    options.TemplateResource = template.ResourcesFolder;
                    options.LastPass = template.NextResource.IsEmpty();
                    options.SchemaName = template.IsGeneral ? schemaNameToFake : schemaName;
                    options.DescriptionsSchemaName = template.IsGeneral ? schemaNameToFake : descriptionsSchemaName;
                    options.TableList = template.IsGeneral ? new List<string>() { "first_table" } : baseOptions.TableList;
                    options.GenerateJsonInfo = !template.IsGeneral && bool.TryParse(SettingsConfiguration["generateJsonInfo"], out var result) && result;
                    options.JsonGeneratedPath = SettingsConfiguration[nameof(options.JsonGeneratedPath)];

                    if (isFirst && !options.GeneateOnlyJson)
                    {
                        template.PreCommands.ForEach(c => commandService.Exec(c, CompiledVars));
                        isFirst = false;
                    }

                    codeGeneratorService.Initialize(options, library, CompiledVars);
                    codeGeneratorService.ProcessTables(template.IsGeneral ? null : (t) => messageService.Info(string.Format(Messages.TableProcessed, t)),
                                                       template.IsGeneral ? null : _alternativeDictionary);

                    if (options.LastPass && !options.GeneateOnlyJson)
                    {
                        template.PostCommands.ForEach(c => commandService.Exec(c, CompiledVars));
                    }

                    template = library.Templates.FirstOrDefault(t => t.ResourcesFolder.Equals(template.NextResource, StringComparison.CurrentCultureIgnoreCase));
                }

                if (!options.GeneateOnlyJson)
                {
                    gitService.GitAdd(path);
                    messageService.Info(Messages.AddedCodeToGit);
                }
            }
            catch (CodeGeneratorException ex)
            {
                messageService.Info(ex.LastError, MessageCodes.RUN_GENERATOR);
                gitService.GitReset(path);
                return;
            }
            catch (Exception ex)
            {
                messageService.Error($"GeneratorAdapter.Run(...) - {Messages.ErrorGeneratingCode} : {ex.Message}", Messages.ErrorGeneratingCode, MessageCodes.RUN_GENERATOR);
                gitService.GitReset(path);
                return;
            }

            messageService.Clear(MessageCodes.RUN_GENERATOR);
            messageService.Clear(MessageCodes.FIND_SERVICE);
            messageService.Info(Messages.CodeGeneratedOK);
        }

        public void SetCurrentDataSource(string selectedDataSource)
        {
            if (selectedDataSource == SelectedDataSource) return;

            SelectedDataSource = selectedDataSource;
            SetupInitial();
            SetupTables();
            SetCurrentModule(GetModuleList(selectedDataSource).FirstOrDefault(), true);
            InputSourceType = schemaFactory.GetProviderDefinitionKey(schemaName);
        }

        public void SetCurrentModule(string selectedModule, bool triggered = false)
        {
            if (selectedModule == SelectedModule && !triggered) return;

            SelectedModule = selectedModule;
            (OutputPath, SolutionType) = GetSavePathFromSelectedTemplate();
            slnFiles = appService.Set(SolutionType).Find(OutputPath);
        }

        public void SetCurrentTemplate(string selectedTemplate, bool triggered = false)
        {
            if (selectedTemplate == SelectedTemplate && !triggered) return;

            SelectedTemplate = selectedTemplate;
            SetCurrentVars(GetVarsList(selectedTemplate).FirstOrDefault(), true);
        }

        public void SetCurrentVars(string selectedVars, bool triggered = false)
        {
            if (selectedVars == SelectedVars && !triggered) return;

            SelectedVars = selectedVars;
            (OutputPath, SolutionType) = GetSavePathFromSelectedTemplate();
            slnFiles = appService.Set(SolutionType).Find(OutputPath);
        }

        #endregion Public Methods

        #region Private Methods

        private void CheckTemplateFileChanges(string obj)
        {
            Thread.Sleep(500);
            jsonSetting.ReloadSettings();
            jsonSetting.AddSettingFiles(SettingsConfiguration["TemplateSettings"]);
            SettingsConfiguration.Merge(jsonSetting.Config[SectionLabels.APP_SETTINGS].ToNameValueCollection());
            SettingsConfiguration["TemplatesBasePath"] ??= Path.GetDirectoryName(SettingsConfiguration["TemplateSettings"]).AddPathSeparator();
            SetCurrentTemplate(SelectedTemplate, true);
            OnConfigurationChanged?.Invoke();
        }

        private bool CheckConnection()
        {
            try
            {
                var canConnectToDb = schemaReader?.CanConnect(includeCommentServer: false) ?? false;
                var canConnectToDbSchema = schemaReader?.CanConnect(includeCommentServer: true) ?? false;
                _alternativeDictionary = schemaService.GetSchemaDictionaryFromFile(descriptionsSchemaName);

                if (canConnectToDb)
                {
                    if (!canConnectToDbSchema)
                    {
                        messageService.Warning(_alternativeDictionary.Any() ? Messages.ErrorDbSchemaButFile : Messages.ErrorDBSchema);
                        return !_alternativeDictionary.Any();
                    }
                    else
                    {
                        messageService.Clear(MessageCodes.CONNECTION);
                        return true;
                    }
                }
                else
                {
                    messageService.Warning(Messages.ErrorConnectToDb, MessageCodes.CONNECTION);
                    return false;
                }
            }
            catch (Exception ex)
            {
                messageService.Error($"GeneratorAdapter.CheckConnection(): {ex.Message}", Messages.ErrorDb, MessageCodes.CONNECTION);
                return false;
            }
        }

        private static void EmptyOutputPath(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            DirectoryInfo directory = new(path);

            foreach (var file in directory.GetFiles().Where(f => !f.Name.StartsWith(".git")))
            {
                file.Delete();
            }

            foreach (var dir in directory.GetDirectories().Where(d => !d.Name.StartsWith(".")))
            {
                dir.Delete(true);
            }
        }

        private (string OutputPath, string SolutionType) GetSavePathFromSelectedTemplate()
        {
            if (SelectedDataSource == null || SelectedModule == null || SelectedTemplate == null)
            {
                return (null, null);
            }

            var basePath = SettingsConfiguration["TemplatesBasePath"].AddPathSeparator();
            var template = TemplatesConfiguration[SelectedTemplate].Path;
            CodeGeneratorDto options = new()
            {
                Area = DataSourcesConfiguration[SelectedDataSource].Area,
                Module = ModulesConfiguration[SelectedModule].Name,
                GenerateReferenced = false,
                OutputPath = null,
                CreatedFromSchemaName = outputSchemaName,
                UserVariables = string.Concat(
                    TemplatesConfiguration[SelectedTemplate].Configurations?[SelectedVars],
                    DataSourcesConfiguration[SelectedDataSource].Vars.ToSemiColonSeparatedString()),
                TemplatePath = Path.Combine(basePath, template),
                TemplateResource = TemplatesConfiguration.Resource(SelectedTemplate),
                SchemaName = schemaNameToFake,
                DescriptionsSchemaName = schemaNameToFake,
                TableList = new List<string>() { "first_table" },
                GeneratorApplication = GeneratorApplication,
                GeneratorVersion = GeneratorVersion,
                Encoding = SettingsConfiguration[nameof(CodeGeneratorDto.Encoding)] ?? "utf8",
                EndOfLine = SettingsConfiguration[nameof(CodeGeneratorDto.EndOfLine)] ?? Environment.NewLine,
                SchemasConfiguration = SchemasConfiguration,
            };

            try
            {
                var library = templateService.TryLoad(options.TemplatePath, options.TemplateResource);
                codeGeneratorService.Initialize(options, library);
                CompiledVars.ClearAndAddRange(codeGeneratorService.GetVars());
                CompiledVars.TryGetValue("SavePath", out var savePath);
                messageService.Clear(MessageCodes.LOADING_TEMPLATES);

                return (savePath as string, codeGeneratorService.GetSolutionType());
            }
            catch (CodeGeneratorException ex)
            {
                messageService.Error(ex.LastError, Messages.ResourceManager.GetString(ex.LastExceptionType.ToString()), MessageCodes.LOADING_TEMPLATES);
            }
            catch (Exception)
            {
                messageService.Info(Messages.ErrorLoadingTemplate, MessageCodes.LOADING_TEMPLATES);
            }

            return (null, null);
        }

        private void SetupInitial()
        {
            schemaName = DataSourcesConfiguration.Schema(SelectedDataSource);
            descriptionsSchemaName = DataSourcesConfiguration.CommentSchema(SelectedDataSource);
            outputSchemaName = DataSourcesConfiguration.OutputSchema(SelectedDataSource);
            schemaReader = schemaFactory.Create(schemaOptionsFactory.Create(SchemasConfiguration, schemaName, writer, descriptionsSchemaName));
            if (schemaReader == null)
            {
                messageService.Error($"GeneratorAdapter.SetupInitial(): {LogService.LastError}", LogService.LastError, MessageCodes.READ_SCHEMA);
                return;
            }
            CheckConnection();
        }

        private void SetupTables()
        {
            if (schemaReader == null)
            {
                tables = new Tables();
                return;
            }

            try
            {
                tables = schemaReader.ReadSchema(new SchemaReaderOptions(schemaService.ShouldRemoveWord1(schemaName), _alternativeDictionary))
                    ?? new Tables();

                messageService.Clear(MessageCodes.READ_SCHEMA);
            }
            catch (Exception ex)
            {
                messageService.Error($"GeneratorAdapter.SetupTables() : {ex.Message}", Messages.ErrorReadingSchema, MessageCodes.READ_SCHEMA);
                tables = new Tables();
            }
        }

        #endregion Private Methods
    }
}