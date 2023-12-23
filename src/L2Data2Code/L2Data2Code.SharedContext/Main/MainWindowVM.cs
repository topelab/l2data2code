using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Main.CommandBar;
using L2Data2Code.SharedContext.Main.MessagePanel;
using L2Data2Code.SharedContext.Main.TablePanel;
using L2Data2CodeUI.Shared.Dto;
using System.Collections.ObjectModel;

namespace L2Data2Code.SharedContext.Main
{
    public class MainWindowVM : ViewModelBase
    {
        private ObservableCollection<string> _areaList = [];
        private ObservableCollection<string> _moduleList = [];
        private ObservableCollection<string> _templateList = [];
        private ObservableCollection<string> _varsList = [];
        private bool _emptyFolders;
        private bool _generateOnlyJson;
        private bool _generateOnlyJsonVisible;
        private bool _havePSInstalled;
        private bool _haveVSCodeInstalled;
        private string _outputPath;
        private bool _runningGenerateCode = false;
        private string _selectedDataSource;
        private string _selectedModule;
        private string _selectedTemplate;
        private string _selectedVars;
        private bool _setRelatedTables;
        private bool _showSettingsWindow;
        private string _slnFile;
        private bool varsVisible = true;
        private AppType appType;
        private CommandBarVM commandBarVM;
        private TablePanelVM tablePanelVM;
        private MessagePanelVM messagePanelVM;

        public AppType AppType { get => appType; internal set => SetProperty(ref appType, value); }
        public ObservableCollection<string> DataSourceList
        {
            get { return _areaList; }
            set { SetProperty(ref _areaList, value); }
        }

        public CommandBarVM CommandBarVM { get => commandBarVM; internal set => SetProperty(ref commandBarVM, value); }
        public TablePanelVM TablePanelVM { get => tablePanelVM; internal set => SetProperty(ref tablePanelVM, value); }
        public MessagePanelVM MessagePanelVM { get => messagePanelVM; internal set => SetProperty(ref messagePanelVM, value); }

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

        public ObservableCollection<string> ModuleList
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
            get { return _selectedTemplate; }
            set { SetProperty(ref _selectedTemplate, value); }
        }

        public string SelectedSetting
        {
            get { return _selectedVars; }
            set { SetProperty(ref _selectedVars, value); }
        }

        public bool SetRelatedTables
        {
            get { return _setRelatedTables; }
            set { SetProperty(ref _setRelatedTables, value); }
        }

        public bool ShowSettingsWindow
        {
            get { return _showSettingsWindow; }
            set { SetProperty(ref _showSettingsWindow, value); }
        }

        public string SlnFile
        {
            get { return _slnFile; }
            set { SetProperty(ref _slnFile, value); }
        }

        public ObservableCollection<string> TemplateList
        {
            get { return _templateList; }
            set { SetProperty(ref _templateList, value); }
        }
        public ObservableCollection<string> Settings
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

        public IDelegateCommand GenerateCodeCommand { get; private set; }

        public void CheckButtonStates()
        {
            OnPropertyChanged(nameof(GenerateCodeCommand));
            OnPropertyChanged(nameof(SlnFile));
        }

        public void SetCommands(IDelegateCommand generateCommand)
        {
            GenerateCodeCommand = generateCommand;
        }
    }
}