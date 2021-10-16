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

namespace L2Data2CodeWPF.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly bool initialGenerateOnlyJsonVisible;
        private readonly IGeneratorAdapter generatorAdapter;
        private readonly IMessagesViewModel messagesViewModel;
        private readonly IMessageService messageService;
        private readonly IAppService appService;
        private readonly IDispatcherWrapper dispatcher;

        public CommandBarViewModel CommandBarViewModel { get; internal set; }
        public TablePanelViewModel TablePanelViewModel { get; internal set; }
        public string GeneratorApplication { get => generatorAdapter.GeneratorApplication; set => generatorAdapter.GeneratorApplication = value; }
        public string GeneratorVersion { get => generatorAdapter.GeneratorVersion; set => generatorAdapter.GeneratorVersion = value; }
        public IGeneratorAdapter Adapter => generatorAdapter;

        private string _selectedtemplate;
        public string SelectedTemplate
        {
            get { return _selectedtemplate; }
            set { SetProperty(ref _selectedtemplate, value, () => TemplateChanged()); }
        }

        private IEnumerable<string> _templateList;
        public IEnumerable<string> TemplateList
        {
            get { return _templateList; }
            set { SetProperty(ref _templateList, value); }
        }

        private string _selectedArea;
        public string SelectedArea
        {
            get { return _selectedArea; }
            set { SetProperty(ref _selectedArea, value, () => AreaChanged()); }
        }

        private IEnumerable<string> _areaList;
        public IEnumerable<string> AreaList
        {
            get { return _areaList; }
            set { SetProperty(ref _areaList, value); }
        }

        private string _selectedVars;
        public string SelectedVars
        {
            get { return _selectedVars; }
            set { SetProperty(ref _selectedVars, value, VarsChanged); }
        }

        private IEnumerable<string> _varsList;
        public IEnumerable<string> VarsList
        {
            get { return _varsList; }
            set { SetProperty(ref _varsList, value); }
        }

        private bool varsVisible = true;
        public bool VarsVisible
        {
            get { return varsVisible; }
            set { SetProperty(ref varsVisible, value); }
        }

        private IEnumerable<string> _moduleList;
        public IEnumerable<string> ModuleList
        {
            get { return _moduleList; }
            set { SetProperty(ref _moduleList, value); }
        }

        private string _selectedModule;
        public string SelectedModule
        {
            get { return _selectedModule; }
            set
            {
                SetProperty(ref _selectedModule, value);
                ModuleChanged();
            }
        }

        private bool _setRelatedTables;
        public bool SetRelatedTables
        {
            get { return _setRelatedTables; }
            set { SetProperty(ref _setRelatedTables, value); }
        }

        private bool _emptyFolders;
        public bool EmptyFolders
        {
            get { return _emptyFolders; }
            set { SetProperty(ref _emptyFolders, value); }
        }

        private bool _generateOnlyJson;
        public bool GenerateOnlyJson
        {
            get { return _generateOnlyJson; }
            set { SetProperty(ref _generateOnlyJson, value); }
        }

        private bool _generateOnlyJsonVisible;

        public bool GenerateOnlyJsonVisible
        {
            get { return _generateOnlyJsonVisible; }
            set { SetProperty(ref _generateOnlyJsonVisible, value); }
        }

        private string _outputPath;

        public string OutputPath
        {
            get { return _outputPath; }
            set { SetProperty(ref _outputPath, value); }
        }

        private string _slnFile;

        public string SlnFile
        {
            get { return _slnFile; }
            set
            {
                SetProperty(ref _slnFile, value, () =>
                {
                    CommandBarViewModel.OnPropertyChanged(nameof(CommandBarViewModel.ChangeButtons));
                    CommandBarViewModel.OnPropertyChanged(nameof(CommandBarViewModel.OpenVSCommand));
                    CommandBarViewModel.OnPropertyChanged(nameof(CommandBarViewModel.OpenFolderCommand));
                    CommandBarViewModel.OnPropertyChanged(nameof(CommandBarViewModel.OpenPSCommand));
                    CommandBarViewModel.OnPropertyChanged(nameof(CommandBarViewModel.OpenVSCodeCommand));
                });
            }
        }

        public bool ChangeButtons { get; set; }

        public AppType AppType => appService.AppType;

        private bool _messagePanelOpened;

        public bool MessagePanelOpened
        {
            get { return _messagePanelOpened; }
            set { SetProperty(ref _messagePanelOpened, value, () => { messagesViewModel.ViewAll(value); }); }
        }

        public bool MessagePanelVisible { get => AllMessages.Any(); }

        private bool _showVarsWindow;

        public bool ShowVarsWindow
        {
            get { return _showVarsWindow; }
            set { SetProperty(ref _showVarsWindow, value); }
        }

        public IMessageService MessageService => messageService;
        public IAppService AppService => appService;


        /// <summary>
        /// Gets the generate code command.
        /// </summary>
        /// <value>
        /// The generate code command.
        /// </value>
        public DelegateCommand GenerateCodeCommand => new(OnGenerateCodeCommand, CanGenerateCodeCommand);

        private bool CanGenerateCodeCommand(object arg)
        {
            if (OutputPath == null)
            {
                return false;
            }
            var existSln = CommandBarViewModel.CheckCanOpenVS(OutputPath, out var slnFile);
            var runnig = ProcessManager.IsRunning(slnFile);
            var anyItems = TablePanelViewModel.AllDataItems.Any(k => k.Value.IsSelected);

            var result = !RunningGenerateCode && (!existSln || (existSln && !runnig)) && anyItems;

            if (!RunningGenerateCode && anyItems && runnig)
            {
                messagesViewModel.Add(string.Format(Strings.CannotGenerateCode, slnFile), MessagePanelOpened, MessageCodes.CAN_GENERATE_CODE);
            }
            else
            {
                messagesViewModel.ClearPinned(MessageCodes.CAN_GENERATE_CODE);
            }
            return result;
        }

        private bool _runningGenerateCode = false;

        private void OnGenerateCodeCommand(object obj)
        {
            Working = true;
            RunningGenerateCode = true;
            CheckButtonStates();

            CodeGeneratorDto options = new()
            {
                GenerateReferenced = TablePanelViewModel.SetRelatedTables,
                OutputPath = OutputPath.AddPathSeparator(),
                RemoveFolders = EmptyFolders,
                TableList = TablePanelViewModel.AllDataItems.Where(k => k.Value.IsSelected).Select(k => k.Key).ToList(),
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

        public ObservableCollection<MessageViewModel> AllMessages => messagesViewModel.AllMessages;
        public bool PauseTimer { get; set; }
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


        private bool _haveVSCodeInstalled;
        public bool HaveVSCodeInstalled
        {
            get { return _haveVSCodeInstalled; }
            set { SetProperty(ref _haveVSCodeInstalled, value); }
        }

        public string VSCodePath { get; set; }

        private bool _havePSInstalled;

        public bool HavePSInstalled
        {
            get { return _havePSInstalled; }
            set { SetProperty(ref _havePSInstalled, value); }
        }

        public string PSPath { get; set; }

        public MainWindowViewModel(IMessagesViewModel messagesViewModel,
                                   IMessageService messageService,
                                   IAppService appService,
                                   IGeneratorAdapter generatorAdapter,
                                   IDispatcherWrapper dispatcher)
        {
            this.generatorAdapter = generatorAdapter;
            this.appService = appService;
            this.messageService = messageService;
            this.messagesViewModel = messagesViewModel;
            this.dispatcher = dispatcher;

            this.messageService.SetActions(ShowMessage, ClearMessages);

            App.Logger.Info("Opening MainWindowViewModel");

            VSCodePath = ProcessManager.FindVSCode();
            HaveVSCodeInstalled = VSCodePath.NotEmpty();

            PSPath = ProcessManager.FindPS();
            HavePSInstalled = PSPath.NotEmpty();

            this.generatorAdapter.GeneratorApplication = Strings.Title;
            this.generatorAdapter.GeneratorVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            CommandBarViewModel = new CommandBarViewModel(this);
            TablePanelViewModel = new TablePanelViewModel(this, dispatcher);

            ShowVarsWindow = bool.TryParse(this.generatorAdapter.SettingsConfiguration["showVarsWindow"], out var showVarsWindow) && showVarsWindow;
            initialGenerateOnlyJsonVisible = bool.TryParse(this.generatorAdapter.SettingsConfiguration["generateJsonInfo"], out var generateJsonInfo) && generateJsonInfo
                && this.generatorAdapter.SettingsConfiguration[nameof(CodeGeneratorDto.JsonGeneratedPath)].NotEmpty();

            EmptyFolders = true;
            TablePanelViewModel.SetRelatedTables = true;

            TemplateList = this.generatorAdapter.GetTemplateList();
            _selectedtemplate = TemplateList.FirstOrDefault();

            VarsList = this.generatorAdapter.GetVarsList(_selectedtemplate);
            _selectedVars = VarsList.FirstOrDefault();

            AreaList = this.generatorAdapter.GetAreaList();
            _selectedArea = AreaList.FirstOrDefault();

            ModuleList = this.generatorAdapter.GetModuleList(_selectedArea);
            _selectedModule = ModuleList.FirstOrDefault();

            TemplateChanged();
            AreaChanged();

            AllMessages.CollectionChanged += AllMessages_CollectionChanged;
            this.generatorAdapter.OnConfigurationChanged = () =>
            {
                TemplateChanged();
                AreaChanged();
            };
        }

        private void ClearMessages(string code)
        {
            messagesViewModel.ClearPinned(code);
        }

        private void ShowMessage(MessageType messageType, string message, string showMessage, string code)
        {
            if (showMessage.NotEmpty())
            {
                messagesViewModel.Add(showMessage, MessagePanelOpened, code);
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

        private void AllMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(MessagePanelVisible));
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

        private void AreaChanged()
        {
            Working = true;
            TablePanelViewModel.LoadingTables = true;
            TablePanelViewModel.ViewsVisible = false;
            Task.Run(() =>
            {
                PauseTimer = true;
                generatorAdapter.SetCurrentArea(SelectedArea);
                TablePanelViewModel.LoadAllTables();
                _moduleList = generatorAdapter.GetModuleList(SelectedArea);
                dispatcher?.Invoke(() =>
                {
                    SelectedModule = ModuleList.FirstOrDefault();
                    OnPropertyChanged(nameof(ModuleList));
                    OutputPath = generatorAdapter.OutputPath;
                    SlnFile = generatorAdapter.SlnFile;
                    GenerateOnlyJsonVisible = initialGenerateOnlyJsonVisible && generatorAdapter.InputSourceType != "json" && generatorAdapter.InputSourceType != "fake";
                });
            }).ContinueWith((t) =>
            {
                dispatcher?.Invoke(() =>
                {
                    TablePanelViewModel.LoadingTables = false;
                    Working = false;
                    PauseTimer = false;
                });
            });
        }

        private void ModuleChanged()
        {
            generatorAdapter.SetCurrentModule(SelectedModule);
            OutputPath = generatorAdapter.OutputPath;
            SlnFile = generatorAdapter.SlnFile;
            if (SelectedModule != null)
            {
                TablePanelViewModel.IncludeExcludeTablesChanged();
            }
        }


        private void VarsChanged()
        {
            generatorAdapter.SetCurrentVars(SelectedVars);
            OutputPath = generatorAdapter.OutputPath;
            SlnFile = generatorAdapter.SlnFile;
        }

        private void CheckButtonStates()
        {
            CommandBarViewModel.OnPropertyChanged(nameof(CommandBarViewModel.OpenVSCommand));
            OnPropertyChanged(nameof(GenerateCodeCommand));
            CommandBarViewModel.OnPropertyChanged(nameof(CommandBarViewModel.OpenFolderCommand));
            CommandBarViewModel.OnPropertyChanged(nameof(CommandBarViewModel.OpenPSCommand));
            CommandBarViewModel.OnPropertyChanged(nameof(CommandBarViewModel.OpenVSCodeCommand));
        }

        public async void CheckOpenedTimerCallBack(object _)
        {
            if (PauseTimer) return;
            PauseTimer = true;

            try
            {
                await ProcessManager.UpdateRunningEditors();
                await ProcessManager.CheckSolutionOpened();
                await ProcessManager.CheckEditorsOpenedAsync(generatorAdapter.SettingsConfiguration["Editor"]);
            }
            catch (Exception ex)
            {
                App.Logger.Error($"MainWindowViewModel.CheckOpenedTimerCallBack(): {ex.Message}");
            }

            CheckButtonStates();

            PauseTimer = false;
        }
    }
}