using L2Data2Code.BaseGenerator.Configuration;
using L2Data2Code.BaseGenerator.Entities;
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
        private ObservableCollection<DataSourceConfiguration> dataSourceList = [];
        private ObservableCollection<ModuleConfiguration> moduleList = [];
        private ObservableCollection<TemplateConfiguration> templateList = [];
        private ObservableCollection<Setting> settingsList = [];
        private bool _emptyFolders;
        private bool _generateOnlyJson;
        private bool _generateOnlyJsonVisible;
        private bool _havePSInstalled;
        private bool _haveVSCodeInstalled;
        private string _outputPath;
        private bool _runningGenerateCode = false;
        private DataSourceConfiguration selectedDataSource;
        private ModuleConfiguration selectedModule;
        private TemplateConfiguration selectedTemplate;
        private Setting selectedSetting;
        private bool _setRelatedTables;
        private bool _showSettingsWindow;
        private string _slnFile;
        private bool varsVisible = true;
        private AppType appType;
        private CommandBarVM commandBarVM;
        private TablePanelVM tablePanelVM;
        private MessagePanelVM messagePanelVM;

        public AppType AppType { get => appType; internal set => SetProperty(ref appType, value); }
        public ObservableCollection<DataSourceConfiguration> DataSourceList
        {
            get { return dataSourceList; }
            set { SetProperty(ref dataSourceList, value); }
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

        public ObservableCollection<ModuleConfiguration> ModuleList
        {
            get { return moduleList; }
            set { SetProperty(ref moduleList, value); }
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

        public DataSourceConfiguration SelectedDataSource
        {
            get { return selectedDataSource; }
            set { SetProperty(ref selectedDataSource, value); }
        }

        public ModuleConfiguration SelectedModule
        {
            get { return selectedModule; }
            set { SetProperty(ref selectedModule, value); }
        }

        public TemplateConfiguration SelectedTemplate
        {
            get { return selectedTemplate; }
            set { SetProperty(ref selectedTemplate, value); }
        }

        public Setting SelectedSetting
        {
            get { return selectedSetting; }
            set { SetProperty(ref selectedSetting, value); }
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

        public ObservableCollection<TemplateConfiguration> TemplateList
        {
            get { return templateList; }
            set { SetProperty(ref templateList, value); }
        }
        public ObservableCollection<Setting> Settings
        {
            get { return settingsList; }
            set { SetProperty(ref settingsList, value); }
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