using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Dto;
using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.Controls.CommandBar;
using L2Data2CodeWPF.Controls.TablePanel;
using L2Data2CodeWPF.SharedLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace L2Data2CodeWPF.Main
{
    public class MainWindowVM : BaseVM
    {
        private readonly IAppService appService;
        private readonly IDispatcherWrapper dispatcher;
        private readonly IGeneratorAdapter generatorAdapter;
        private readonly bool initialGenerateOnlyJsonVisible;
        private readonly IMessagePanelService messagePanelService;
        private readonly IMessageService messageService;
        private readonly IProcessManager processManager;

        private IEnumerable<string> _areaList;
        private bool _emptyFolders;
        private bool _generateOnlyJson;
        private bool _generateOnlyJsonVisible;
        private bool _havePSInstalled;
        private bool _haveVSCodeInstalled;
        private bool _messagePanelOpened;
        private IEnumerable<string> _moduleList;
        private string _outputPath;
        private bool _runningGenerateCode = false;
        private string _selectedDataSource;
        private string _selectedModule;
        private string _selectedtemplate;
        private string _selectedVars;
        private bool _setRelatedTables;
        private bool _showVarsWindow;
        private string _slnFile;
        private IEnumerable<string> _templateList;
        private IEnumerable<string> _varsList;
        private bool varsVisible = true;

        public MainWindowVM(IMessagePanelService messagePanelService,
                            IMessageService messageService,
                            IAppService appService,
                            IGeneratorAdapter generatorAdapter,
                            IDispatcherWrapper dispatcher,
                            IProcessManager processManager)
        {
            this.generatorAdapter = generatorAdapter;
            this.appService = appService;
            this.messageService = messageService;
            this.messagePanelService = messagePanelService;
            this.dispatcher = dispatcher;
            this.processManager = processManager;

            this.messageService.SetActions(ShowMessage, ClearMessages);

            App.Logger.Info($"Opening {nameof(MainWindowVM)}");

            VSCodePath = processManager.FindVSCode();
            HaveVSCodeInstalled = VSCodePath.NotEmpty();

            PSPath = processManager.FindPS();
            HavePSInstalled = PSPath.NotEmpty();

            this.generatorAdapter.GeneratorApplication = Strings.Title;
            this.generatorAdapter.GeneratorVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            CommandBarVM = new CommandBarVM(this);
            TablePanelVM = new TablePanelVM(this);

            ShowVarsWindow = bool.TryParse(this.generatorAdapter.SettingsConfiguration["showVarsWindow"], out var showVarsWindow) && showVarsWindow;
            initialGenerateOnlyJsonVisible = bool.TryParse(this.generatorAdapter.SettingsConfiguration["generateJsonInfo"], out var generateJsonInfo) && generateJsonInfo
                && this.generatorAdapter.SettingsConfiguration[nameof(CodeGeneratorDto.JsonGeneratedPath)].NotEmpty();

            EmptyFolders = true;
            TablePanelVM.SetRelatedTables = true;

            TemplateList = this.generatorAdapter.GetTemplateList();
            _selectedtemplate = TemplateList.FirstOrDefault();

            VarsList = this.generatorAdapter.GetVarsList(_selectedtemplate);
            _selectedVars = VarsList.FirstOrDefault();

            DataSourceList = this.generatorAdapter.GetAreaList();
            _selectedDataSource = DataSourceList.FirstOrDefault();

            ModuleList = this.generatorAdapter.GetModuleList(_selectedDataSource);
            _selectedModule = this.generatorAdapter.GetDefaultModule(_selectedDataSource);

            TemplateChanged();
            AreaChanged();

            AllMessages.CollectionChanged += AllMessages_CollectionChanged;
            this.generatorAdapter.OnConfigurationChanged = () =>
            {
                TemplateChanged();
                AreaChanged();
            };
        }

        public IGeneratorAdapter Adapter => generatorAdapter;
        public ObservableCollection<MessageVM> AllMessages => messagePanelService.AllMessages;
        public IAppService AppService => appService;
        public AppType AppType => appService.AppType;
        public IEnumerable<string> DataSourceList
        {
            get { return _areaList; }
            set { SetProperty(ref _areaList, value); }
        }

        public bool ChangeButtons { get; set; }
        public CommandBarVM CommandBarVM { get; internal set; }
        public IDispatcherWrapper Dispatcher => dispatcher;
        public bool EmptyFolders
        {
            get { return _emptyFolders; }
            set { SetProperty(ref _emptyFolders, value); }
        }

        /// <summary>
        /// Gets the generate code command.
        /// </summary>
        /// <value>
        /// The generate code command.
        /// </value>
        public DelegateCommand GenerateCodeCommand => new(OnGenerateCodeCommand, CanGenerateCodeCommand);

        public bool GenerateOnlyJson
        {
            get { return _generateOnlyJson; }
            set { SetProperty(ref _generateOnlyJson, value); }
        }

        public bool GenerateOnlyJsonVisible
        {
            get { return _generateOnlyJsonVisible; }
            set { SetProperty(ref _generateOnlyJsonVisible, value); }
        }

        public string GeneratorApplication { get => generatorAdapter.GeneratorApplication; set => generatorAdapter.GeneratorApplication = value; }
        public string GeneratorVersion { get => generatorAdapter.GeneratorVersion; set => generatorAdapter.GeneratorVersion = value; }
        public bool HavePSInstalled
        {
            get { return _havePSInstalled; }
            set { SetProperty(ref _havePSInstalled, value); }
        }

        public bool HaveVSCodeInstalled
        {
            get { return _haveVSCodeInstalled; }
            set { SetProperty(ref _haveVSCodeInstalled, value); }
        }

        public bool MessagePanelOpened
        {
            get { return _messagePanelOpened; }
            set { SetProperty(ref _messagePanelOpened, value, () => { messagePanelService.ViewAll(value); }); }
        }

        public bool MessagePanelVisible { get => AllMessages.Any(); }
        public IMessageService MessageService => messageService;
        public IEnumerable<string> ModuleList
        {
            get { return _moduleList; }
            set { SetProperty(ref _moduleList, value); }
        }

        public string OutputPath
        {
            get { return _outputPath; }
            set { SetProperty(ref _outputPath, value); }
        }

        public bool PauseTimer { get; set; }
        public IProcessManager ProcessManager => processManager;
        public string PSPath { get; set; }
        public bool RunningGenerateCode
        {
            get => _runningGenerateCode;
            set
            {
                _runningGenerateCode = value;
                if (value == true)
                {
                    OnPropertyChanged();
                }
            }
        }

        public string SelectedDataSource
        {
            get { return _selectedDataSource; }
            set { SetProperty(ref _selectedDataSource, value, () => AreaChanged()); }
        }

        public string SelectedModule
        {
            get { return _selectedModule; }
            set
            {
                SetProperty(ref _selectedModule, value);
                ModuleChanged();
            }
        }

        public string SelectedTemplate
        {
            get { return _selectedtemplate; }
            set { SetProperty(ref _selectedtemplate, value, () => TemplateChanged()); }
        }

        public string SelectedVars
        {
            get { return _selectedVars; }
            set { SetProperty(ref _selectedVars, value, VarsChanged); }
        }

        public bool SetRelatedTables
        {
            get { return _setRelatedTables; }
            set { SetProperty(ref _setRelatedTables, value); }
        }

        public bool ShowVarsWindow
        {
            get { return _showVarsWindow; }
            set { SetProperty(ref _showVarsWindow, value); }
        }

        public string SlnFile
        {
            get { return _slnFile; }
            set
            {
                SetProperty(ref _slnFile, value, () =>
                {
                    CommandBarVM.OnPropertyChanged(nameof(CommandBarVM.ChangeButtons));
                    CommandBarVM.OnPropertyChanged(nameof(CommandBarVM.OpenVSCommand));
                    CommandBarVM.OnPropertyChanged(nameof(CommandBarVM.OpenFolderCommand));
                    CommandBarVM.OnPropertyChanged(nameof(CommandBarVM.OpenPSCommand));
                    CommandBarVM.OnPropertyChanged(nameof(CommandBarVM.OpenVSCodeCommand));
                });
            }
        }

        public TablePanelVM TablePanelVM { get; internal set; }
        public IEnumerable<string> TemplateList
        {
            get { return _templateList; }
            set { SetProperty(ref _templateList, value); }
        }
        public IEnumerable<string> VarsList
        {
            get { return _varsList; }
            set { SetProperty(ref _varsList, value); }
        }
        public bool VarsVisible
        {
            get { return varsVisible; }
            set { SetProperty(ref varsVisible, value); }
        }
        public string VSCodePath { get; set; }

        public async void CheckOpenedTimerCallBack(object _)
        {
            if (PauseTimer)
            {
                return;
            }

            PauseTimer = true;

            try
            {
                await processManager.UpdateRunningEditors();
                await processManager.CheckSolutionOpened();
                await processManager.CheckEditorsOpenedAsync(generatorAdapter.SettingsConfiguration["Editor"]);
            }
            catch (Exception ex)
            {
                App.Logger.Error($"{nameof(MainWindowVM)}.{nameof(CheckOpenedTimerCallBack)}(): {ex.Message}");
            }

            CheckButtonStates();

            PauseTimer = false;
        }

        private void AllMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(MessagePanelVisible));
        }

        private void AreaChanged()
        {
            Working = true;
            TablePanelVM.LoadingTables = true;
            TablePanelVM.ViewsVisible = false;
            Task.Run(() =>
            {
                PauseTimer = true;
                generatorAdapter.SetCurrentDataSource(SelectedDataSource);
                TablePanelVM.LoadAllTables();
                _moduleList = generatorAdapter.GetModuleList(SelectedDataSource);
                dispatcher?.Invoke(() =>
                {
                    SelectedModule = generatorAdapter.SelectedModule;
                    OnPropertyChanged(nameof(ModuleList));
                    OutputPath = generatorAdapter.OutputPath;
                    SlnFile = generatorAdapter.SlnFile;
                    GenerateOnlyJsonVisible = initialGenerateOnlyJsonVisible && generatorAdapter.InputSourceType != "json" && generatorAdapter.InputSourceType != "fake";
                });
            }).ContinueWith((t) =>
            {
                dispatcher?.Invoke(() =>
                {
                    TablePanelVM.LoadingTables = false;
                    Working = false;
                    PauseTimer = false;
                });
            });
        }

        private bool CanGenerateCodeCommand(object arg)
        {
            if (OutputPath == null)
            {
                return false;
            }
            var existSln = CommandBarVM.CheckCanOpenVS(OutputPath, out var slnFile);
            var runnig = processManager.IsRunning(slnFile);
            var anyItems = TablePanelVM.AllDataItems.Any(k => k.Value.IsSelected);

            var result = !RunningGenerateCode && (!existSln || existSln && !runnig) && anyItems;

            if (!RunningGenerateCode && anyItems && runnig)
            {
                messagePanelService.Add(string.Format(Strings.CannotGenerateCode, slnFile), MessagePanelOpened, MessageCodes.CAN_GENERATE_CODE);
            }
            else
            {
                messagePanelService.ClearPinned(MessageCodes.CAN_GENERATE_CODE);
            }
            return result;
        }
        private void CheckButtonStates()
        {
            CommandBarVM.OnPropertyChanged(nameof(CommandBarVM.OpenVSCommand));
            OnPropertyChanged(nameof(GenerateCodeCommand));
            CommandBarVM.OnPropertyChanged(nameof(CommandBarVM.OpenFolderCommand));
            CommandBarVM.OnPropertyChanged(nameof(CommandBarVM.OpenPSCommand));
            CommandBarVM.OnPropertyChanged(nameof(CommandBarVM.OpenVSCodeCommand));
        }

        private void ClearMessages(string code)
        {
            messagePanelService.ClearPinned(code);
        }

        private void ModuleChanged()
        {
            generatorAdapter.SetCurrentModule(SelectedModule);
            OutputPath = generatorAdapter.OutputPath;
            SlnFile = generatorAdapter.SlnFile;
            if (SelectedModule != null)
            {
                TablePanelVM.IncludeExcludeTablesChanged();
            }
        }

        private void OnGenerateCodeCommand(object obj)
        {
            Working = true;
            RunningGenerateCode = true;
            CheckButtonStates();

            CodeGeneratorDto options = new()
            {
                GenerateReferenced = TablePanelVM.SetRelatedTables,
                OutputPath = OutputPath.AddPathSeparator(),
                RemoveFolders = EmptyFolders,
                TableList = TablePanelVM.AllDataItems.Where(k => k.Value.IsSelected).Select(k => k.Key).ToList(),
                GeneratorApplication = GeneratorApplication,
                GeneratorVersion = GeneratorVersion,
                GeneateOnlyJson = GenerateOnlyJson
            };
            Task.Run(() => generatorAdapter.Run(options))
                .ContinueWith((state) =>
                {
                    RunningGenerateCode = false;
                    CheckButtonStates();
                    Working = false;
                });

        }
        private void ShowMessage(MessageType messageType, string message, string showMessage, string code)
        {
            if (showMessage.NotEmpty())
            {
                messagePanelService.Add(showMessage, MessagePanelOpened, code);
            }

            if (message.NotEmpty())
            {
                switch (messageType)
                {
                    case MessageType.Info:
                        App.Logger.Info(message);
                        break;
                    case MessageType.Warning:
                        App.Logger.Warn(message);
                        break;
                    case MessageType.Error:
                        App.Logger.Error(message);
                        break;
                    default:
                        break;
                }
            }
        }
        private void TemplateChanged(bool triggered = false)
        {
            Working = true;
            generatorAdapter.SetCurrentTemplate(SelectedTemplate, triggered);
            _varsList = generatorAdapter.GetVarsList(SelectedTemplate);
            SelectedVars = VarsList.FirstOrDefault();
            OnPropertyChanged(nameof(VarsList));
            VarsVisible = SelectedVars != null;
            EmptyFolders = generatorAdapter.TemplatesConfiguration.HasToRemoveFolders(SelectedTemplate);
            OutputPath = generatorAdapter.OutputPath;
            SlnFile = generatorAdapter.SlnFile;

            Working = false;
        }
        private void VarsChanged()
        {
            generatorAdapter.SetCurrentVars(SelectedVars);
            OutputPath = generatorAdapter.OutputPath;
            SlnFile = generatorAdapter.SlnFile;
        }
    }
}