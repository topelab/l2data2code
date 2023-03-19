using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Dto;
using L2Data2CodeUI.Shared.Localize;
using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.Controls.CommandBar;
using L2Data2CodeWPF.Controls.MessagePanel;
using L2Data2CodeWPF.Controls.TablePanel;
using L2Data2CodeWPF.SharedLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace L2Data2CodeWPF.Main
{
    internal class MainWindowVM : BaseVM
    {
        private readonly IDispatcherWrapper dispatcher;
        private readonly IGeneratorAdapter generatorAdapter;
        private readonly IMessagePanelService messagePanelService;
        private readonly IProcessManager processManager;
        private IEnumerable<string> _areaList;
        private bool _emptyFolders;
        private bool _generateOnlyJson;
        private bool _generateOnlyJsonVisible;
        private bool _havePSInstalled;
        private bool _haveVSCodeInstalled;
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
        private AppType appType;

        public MainWindowVM(IMessagePanelService messagePanelService,
                            IGeneratorAdapter generatorAdapter,
                            IDispatcherWrapper dispatcher,
                            IProcessManager processManager)
        {
            this.generatorAdapter = generatorAdapter;
            this.messagePanelService = messagePanelService;
            this.dispatcher = dispatcher;
            this.processManager = processManager;

        }

        public IDispatcherWrapper Dispatcher => dispatcher;
        public AppType AppType { get => appType; internal set => SetProperty(ref appType, value); }
        public IEnumerable<string> DataSourceList
        {
            get { return _areaList; }
            set { SetProperty(ref _areaList, value); }
        }


        public CommandBarVM CommandBarVM { get; internal set; }
        public TablePanelVM TablePanelVM { get; internal set; }
        public MessagePanelVM MessagePanelVM { get; internal set; }

        public bool EmptyFolders
        {
            get { return _emptyFolders; }
            set { SetProperty(ref _emptyFolders, value); }
        }

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
        public string PSPath { get; set; }

        public bool RunningGenerateCode
        {
            get => _runningGenerateCode;
            set => SetProperty(ref _runningGenerateCode, value);
        }

        public string SelectedDataSource
        {
            get { return _selectedDataSource; }
            set { SetProperty(ref _selectedDataSource, value); }
        }

        public string SelectedModule
        {
            get { return _selectedModule; }
            set { SetProperty(ref _selectedModule, value); }
        }

        public string SelectedTemplate
        {
            get { return _selectedtemplate; }
            set { SetProperty(ref _selectedtemplate, value); }
        }

        public string SelectedVars
        {
            get { return _selectedVars; }
            set { SetProperty(ref _selectedVars, value); }
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
            set { SetProperty(ref _slnFile, value); }
        }

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

        public DelegateCommand GenerateCodeCommand => new(OnGenerateCodeCommand, CanGenerateCodeCommand);


        private bool CanGenerateCodeCommand(object arg)
        {
            if (OutputPath == null)
            {
                return false;
            }
            var existSln = File.Exists(SlnFile);
            var runnig = processManager.IsRunning(SlnFile);
            var anyItems = TablePanelVM.AllDataItems.Any(k => k.Value.IsSelected);

            var result = !RunningGenerateCode && (!existSln || existSln && !runnig) && anyItems;

            if (!RunningGenerateCode && anyItems && runnig)
            {
                messagePanelService.Add(string.Format(Strings.CannotGenerateCode, SlnFile), MessagePanelVM.MessagePanelOpened, MessageCodes.CAN_GENERATE_CODE);
            }
            else
            {
                messagePanelService.ClearPinned(MessageCodes.CAN_GENERATE_CODE);
            }
            return result;
        }
        public void CheckButtonStates()
        {
            OnPropertyChanged(nameof(GenerateCodeCommand));
            OnPropertyChanged(nameof(SlnFile));
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
    }
}