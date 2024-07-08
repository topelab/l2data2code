using L2Data2Code.BaseGenerator.Configuration;
using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Exceptions;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.SchemaReader.Configuration;
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
using System.Reflection;
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
        private string outputSchemaName = "localserver";
        private ISchemaReader schemaReader;
        private IEnumerable<string> slnFiles;

        public AppType AppType { get; private set; }

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

            GeneratorApplication = Strings.Title;
            GeneratorVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.Piece(0,'+');

            SettingsConfiguration.Merge(SettingsConfiguration[ConfigurationLabels.TEMPLATE_SETTINGS]);
            SettingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH] ??= Path.GetDirectoryName(SettingsConfiguration[ConfigurationLabels.TEMPLATE_SETTINGS]).AddPathSeparator();

            this.fileMonitorService.StartMonitoring(CheckTemplateFileChanges, SettingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH], "*.json");
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
        public DataSourceConfiguration SelectedDataSource { get; private set; }
        public ModuleConfiguration SelectedModule { get; private set; }
        public TemplateConfiguration SelectedTemplate { get; private set; }
        public Setting SelectedSetting { get; private set; }
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

        public IEnumerable<DataSourceConfiguration> GetAreaList()
            => DataSourcesConfiguration.GetValues();

        public IEnumerable<ModuleConfiguration> GetModuleList(DataSourceConfiguration selectedDataSource)
        {
            if (selectedDataSource.Modules == null)
            {
                if (selectedDataSource.Settings == null)
                {
                    selectedDataSource.Modules = [new ModuleConfiguration(selectedDataSource.DefaultModule)];
                }
                else
                {
                    selectedDataSource.Modules = selectedDataSource.Settings.AllKeys.Select(k => selectedDataSource.Settings[k]).Distinct().Select(k => new ModuleConfiguration(k)).ToList();
                }
            }

            return selectedDataSource.Modules;
        }

        public ModuleConfiguration GetDefaultModule(DataSourceConfiguration selectedDataSource)
        {
            var defaultModule = selectedDataSource.DefaultModule;
            var moduleList = GetModuleList(selectedDataSource);
            return moduleList.FirstOrDefault(m => defaultModule == null || m.Key == defaultModule);
        }

        public IEnumerable<TemplateConfiguration> GetTemplateList()
            => TemplatesConfiguration.GetValues();

        public IEnumerable<Setting> GetSettings(TemplateConfiguration selectedTemplate, DataSourceConfiguration selectedDataSource = null)
        {
            IEnumerable<Setting> selectedSettings = [];

            if (selectedTemplate.Settings?.Count != 0)
            {
                var allSettings = selectedTemplate.Settings;
                var dataSourceSettings = selectedDataSource == null ? null : selectedDataSource.Settings;
                var dataSourceSelectableSettings = dataSourceSettings == null
                        ? []
                        : dataSourceSettings.AllKeys.Select(k => allSettings.FirstOrDefault(s => s.Key == k)).Where(r => r is not null);

                selectedSettings = selectedDataSource == null || !dataSourceSelectableSettings.Any()
                    ? selectedTemplate.Settings
                    : dataSourceSelectableSettings;

            }

            return selectedSettings;
        }

        public void Run(CodeGeneratorDto baseOptions)
        {
            messageService.Clear(MessageCodes.RUN_COMMAND);

            if (!Directory.Exists(baseOptions.OutputPath))
            {
                Directory.CreateDirectory(baseOptions.OutputPath);
            }
            gitService.GitInit(baseOptions.OutputPath);

            var basePath = SettingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH].AddPathSeparator();
            var templatePath = SelectedTemplate.Path;
            CodeGeneratorDto options = new()
            {
                Area = SelectedDataSource.Area,
                Module = SelectedModule.Name,
                GenerateReferenced = baseOptions.GenerateReferenced,
                RemoveFolders = baseOptions.RemoveFolders && !baseOptions.GeneateOnlyJson,
                OutputPath = baseOptions.OutputPath.AddPathSeparator(),
                CreatedFromSchemaName = outputSchemaName,
                UserVariables = string.Concat(
                    SelectedDataSource.Vars.ToSemiColonSeparatedString(),
                    SelectedSetting?.Vars.ToSemiColonSeparatedString()
                    ),
                TemplatePath = Path.Combine(basePath, templatePath),
                TemplateResource = SelectedTemplate.ResourcesFolder ?? Constants.GeneralResourceFolder,
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

                Template lastTemplate = null;
                var template = library.Templates.FirstOrDefault(t => t.ResourcesFolder.Equals(options.TemplateResource, StringComparison.CurrentCultureIgnoreCase));

                if (!options.GeneateOnlyJson)
                {
                    template.PreCommands.ForEach(c => commandService.Exec(c, CompiledVars));
                }

                while (template != null)
                {
                    if (processedTemplates.Contains(template.ResourcesFolder))
                    {
                        break;
                    }
                    processedTemplates.Add(template.ResourcesFolder);

                    options.Template = template.Name;
                    options.TemplateResource = template.ResourcesFolder;
                    options.LastPass = template.NextResource.IsEmpty();
                    options.SchemaName = template.IsGeneral ? schemaNameToFake : schemaName;
                    options.TableList = template.IsGeneral ? new List<string>() { "first_table" } : baseOptions.TableList;
                    options.GenerateJsonInfo = !template.IsGeneral && bool.TryParse(SettingsConfiguration["generateJsonInfo"], out var result) && result;
                    options.JsonGeneratedPath = SettingsConfiguration[nameof(options.JsonGeneratedPath)];

                    codeGeneratorService.Initialize(options, library, CompiledVars);
                    codeGeneratorService.ProcessTables(template.IsGeneral ? null : (t) => messageService.Info(string.Format(Messages.TableProcessed, t)),
                                                       template.IsGeneral ? null : _alternativeDictionary);

                    lastTemplate = template;
                    template = library.Templates.FirstOrDefault(t => t.ResourcesFolder.Equals(template.NextResource, StringComparison.CurrentCultureIgnoreCase));
                }

                if (!options.GeneateOnlyJson)
                {
                    gitService.GitAdd(path);
                    messageService.Info(Messages.AddedCodeToGit);
                    lastTemplate.PostCommands.ForEach(c => commandService.Exec(c, CompiledVars));
                }

                TryWriteDescriptions();
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

        public void SetCurrentDataSource(DataSourceConfiguration selectedDataSource)
        {
            if (selectedDataSource?.Key == SelectedDataSource?.Key)
            {
                return;
            }

            SelectedDataSource = selectedDataSource;
            SetupInitial();
            SetupTables();
            SetCurrentModule(GetDefaultModule(selectedDataSource), true);
            InputSourceType = schemaFactory.GetProviderDefinitionKey(schemaName);
        }

        public void SetCurrentModule(ModuleConfiguration selectedModule, bool triggered = false)
        {
            if (selectedModule?.Key == SelectedModule?.Key && !triggered)
            {
                return;
            }

            SelectedModule = selectedModule;
            (OutputPath, SolutionType) = GetSavePathFromSelectedTemplate();
            slnFiles = appService.Set(SolutionType).Find(OutputPath);
            AppType = appService.AppType;
        }

        public void SetCurrentTemplate(TemplateConfiguration selectedTemplate, bool triggered = false)
        {
            if (selectedTemplate?.Name == SelectedTemplate?.Name && !triggered)
            {
                return;
            }

            SelectedTemplate = selectedTemplate;
            SetCurrentSetting(GetSettings(selectedTemplate, SelectedDataSource).FirstOrDefault(), true);
        }

        public void SetCurrentSetting(Setting selectedSetting, bool triggered = false)
        {
            if (selectedSetting == SelectedSetting && !triggered)
            {
                return;
            }

            SelectedSetting = selectedSetting;
            if (SelectedDataSource != null && selectedSetting != null)
            {
                var dataSourceSettings = SelectedDataSource.Settings;
                var allSettings = SelectedTemplate.Settings;
                if (dataSourceSettings != null && allSettings != null)
                {
                    var key = allSettings.Find(s => s.Name == selectedSetting.Name)?.Key;
                    if (key != null)
                    {
                        var moduleKey = SelectedDataSource.Settings[key];
                        SelectedModule = SelectedDataSource.Modules.FirstOrDefault(m => m.Key == moduleKey)
                            ?? SelectedModule;
                    }
                }
            }
            (OutputPath, SolutionType) = GetSavePathFromSelectedTemplate();
            slnFiles = appService.Set(SolutionType).Find(OutputPath);
            AppType = appService.AppType;
        }

        #endregion Public Methods

        #region Private Methods

        private void CheckTemplateFileChanges(string obj)
        {
            Thread.Sleep(500);
            jsonSetting.ReloadSettings();
            SettingsConfiguration.Merge(SettingsConfiguration[ConfigurationLabels.TEMPLATE_SETTINGS]);
            SettingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH] ??= Path.GetDirectoryName(SettingsConfiguration[ConfigurationLabels.TEMPLATE_SETTINGS]).AddPathSeparator();
            SetCurrentTemplate(SelectedTemplate, true);
            OnConfigurationChanged?.Invoke();
        }

        private bool CheckConnection()
        {
            try
            {
                var canConnectToDb = schemaReader?.CanConnect() ?? false;

                if (canConnectToDb)
                {
                    messageService.Clear(MessageCodes.CONNECTION);
                    return true;
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

            var basePath = SettingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH].AddPathSeparator();
            var template = SelectedTemplate.Path;
            CodeGeneratorDto options = new()
            {
                Area = SelectedDataSource.Area,
                Module = SelectedModule.Name,
                Template = SelectedTemplate.Name,
                GenerateReferenced = false,
                OutputPath = null,
                CreatedFromSchemaName = outputSchemaName,
                UserVariables = string.Concat(
                    SelectedDataSource.Vars.ToSemiColonSeparatedString(),
                    SelectedSetting?.Vars.ToSemiColonSeparatedString()
                    ),
                TemplatePath = Path.Combine(basePath, template),
                TemplateResource = SelectedTemplate.ResourcesFolder ?? Constants.GeneralResourceFolder,
                SchemaName = schemaNameToFake,
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
            var basePath = SettingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH].AddPathSeparator();
            var templatePath = Path.Combine(basePath, SelectedTemplate.Path);

            schemaName = SelectedDataSource.Schema;
            _alternativeDictionary = schemaService.GetSchemaDictionaryFromFile(schemaName, templatePath);
            outputSchemaName = SelectedDataSource.OutputSchema;
            schemaReader = schemaFactory.Create(schemaOptionsFactory.Create(templatePath, SchemasConfiguration, schemaName, writer));
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

        private void TryWriteDescriptions()
        {
            if (SchemasConfiguration[schemaName].WriteDescriptionsFile)
            {
                var basePath = SettingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH].AddPathSeparator();
                var templatePath = Path.Combine(basePath, SelectedTemplate.Path);
                var descriptionFilePath = Path.Combine(templatePath, $"dataSource\\{schemaName.ToSlug()}-descriptions.txt");

                var result = string.Join("\n",
                    tables.Values
                    .SelectMany(t => t.Columns)
                    .OrderBy(c => c.TableName)
                    .ThenBy(c => c.Name)
                    .Select(c => string.Concat(string.Concat(c.TableName, ".", c.Name).PadRight(69), c.Description)));

                File.WriteAllText(descriptionFilePath, string.Concat(result, "\n"));
            }
        }

        #endregion Private Methods
    }
}