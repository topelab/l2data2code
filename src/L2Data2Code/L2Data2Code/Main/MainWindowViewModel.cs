using System.Collections.Generic;
using System.Collections.ObjectModel;
using L2Data2Code.Main.CommandBar;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.ViewModels;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Dto;

namespace L2Data2Code.Main
{
    public class MainWindowViewModel : ViewModelBase
    {
        public IGeneratorAdapter Adapter { get; }
        public ObservableCollection<object> AllMessages { get; }
        public IAppService AppService { get; }
        public AppType AppType { get; }
        public bool ChangeButtons { get; set; }
        public CommandBarVM CommandBarVM { get; }
        public IEnumerable<string> DataSourceList { get; set; }
        public object Dispatcher { get; }
        public bool EmptyFolders { get; set; }
        public object GenerateCodeCommand { get; }
        public bool GenerateOnlyJson { get; set; }
        public bool GenerateOnlyJsonVisible { get; set; }
        public string GeneratorApplication { get; set; }
        public string GeneratorVersion { get; set; }
        public bool HavePSInstalled { get; set; }
        public bool HaveVSCodeInstalled { get; set; }
        public bool MessagePanelOpened { get; set; }
        public bool MessagePanelVisible { get; }
        public IMessageService MessageService { get; }
        public IEnumerable<string> ModuleList { get; set; }
        public string OutputPath { get; set; }
        public bool PauseTimer { get; set; }
        public IProcessManager ProcessManager { get; }
        public string PSPath { get; set; }
        public bool RunningGenerateCode { get; set; }
        public string SelectedDataSource { get; set; }
        public string SelectedModule { get; set; }
        public string SelectedTemplate { get; set; }
        public string SelectedVars { get; set; }
        public bool SetRelatedTables { get; set; }
        public bool ShowVarsWindow { get; set; }
        public string SlnFile { get; set; }
        public ViewModelBase TablePanelVM { get; }
        public IEnumerable<string> TemplateList { get; set; }
        public IEnumerable<string> VarsList { get; set; }
        public bool VarsVisible { get; set; }
        public string VSCodePath { get; set; }
    }
}
