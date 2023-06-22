using L2Data2Code.MAUI.Main.Interfaces;
using L2Data2CodeUI.Shared.Adapters;
using System.ComponentModel;
using System.Windows.Input;

namespace L2Data2Code.MAUI.Main
{
    internal class MainPageVMBindManager : IMainPageVMBindManager
    {
        private readonly IGeneratorAdapter generatorAdapter;

        public MainPageVMBindManager(IGeneratorAdapter generatorAdapter)
        {
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
        }

        public void Start(MainPageVM mainWindowVM)
        {
            mainWindowVM.PropertyChanged += OnMainPageVMPropertyChanged;
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        private void OnMainPageVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var mainPageVM = sender as MainPageVM;
            switch (e.PropertyName)
            {
                case nameof(MainPageVM.EmptyFolders):
                    break;
                case nameof(MainPageVM.GenerateOnlyJson):
                    break;
                case nameof(MainPageVM.GenerateOnlyJsonVisible):
                    break;
                case nameof(MainPageVM.HavePSInstalled):
                    break;
                case nameof(MainPageVM.HaveVSCodeInstalled):
                    break;
                case nameof(MainPageVM.ModuleList):
                    break;
                case nameof(MainPageVM.OutputPath):
                    CommandManager.InvalidateRequerySuggested();
                    break;
                case nameof(MainPageVM.PauseTimer):
                    break;
                case nameof(MainPageVM.RunningGenerateCode):
                    mainPageVM.OnPropertyChanged(nameof(mainPageVM.GenerateCodeCommand));
                    break;
                case nameof(MainPageVM.SelectedDataSource):
                    AreaChanged(mainPageVM);
                    break;
                case nameof(MainPageVM.SelectedModule):
                    ModuleChanged(mainPageVM);
                    break;
                case nameof(MainPageVM.SelectedTemplate):
                    TemplateChanged(mainPageVM);
                    break;
                case nameof(MainPageVM.SelectedVars):
                    VarsChanged(mainPageVM);
                    break;
                case nameof(MainPageVM.SetRelatedTables):
                    break;
                case nameof(MainPageVM.ShowVarsWindow):
                    break;
                case nameof(MainPageVM.SlnFile):
                    mainPageVM.OnPropertyChanged(nameof(mainPageVM.GenerateCodeCommand));
                    break;
                case nameof(MainPageVM.TemplateList):
                    break;
                case nameof(MainPageVM.VarsList):
                    break;
                case nameof(MainPageVM.VarsVisible):
                    break;
                default:
                    break;
            }
        }

        private void AreaChanged(MainPageVM mainPageVM)
        {
            mainPageVM.Working = true;
            mainPageVM.TablePanelVM.LoadingTables = true;
            mainPageVM.TablePanelVM.ViewsVisible = false;
            var initialGenerateOnlyJsonVisible = bool.TryParse(generatorAdapter.SettingsConfiguration["generateJsonInfo"], out var generateJsonInfo) && generateJsonInfo;

            mainPageVM.PauseTimer = true;
            generatorAdapter.SetCurrentDataSource(mainPageVM.SelectedDataSource);
            mainPageVM.TablePanelVM.LoadTablesCommand.Execute(null);
            mainPageVM.ModuleList = generatorAdapter.GetModuleList(mainPageVM.SelectedDataSource);
            mainPageVM.SelectedModule = generatorAdapter.GetDefaultModule(mainPageVM.SelectedDataSource);
            mainPageVM.VarsList = generatorAdapter.GetVarsList(mainPageVM.SelectedTemplate, mainPageVM.SelectedDataSource);
            mainPageVM.SelectedVars = mainPageVM.VarsList.FirstOrDefault();

            mainPageVM.OutputPath = generatorAdapter.OutputPath;
            mainPageVM.SlnFile = generatorAdapter.SlnFile;
            mainPageVM.GenerateOnlyJsonVisible = initialGenerateOnlyJsonVisible && generatorAdapter.InputSourceType != "json" && generatorAdapter.InputSourceType != "fake";
            mainPageVM.TablePanelVM.LoadingTables = false;
            mainPageVM.Working = false;
            mainPageVM.PauseTimer = false;
            CommandManager.InvalidateRequerySuggested();
        }

        private void TemplateChanged(MainPageVM mainPageVM)
        {
            mainPageVM.Working = true;
            generatorAdapter.SetCurrentTemplate(mainPageVM.SelectedTemplate);
            mainPageVM.VarsList = generatorAdapter.GetVarsList(mainPageVM.SelectedTemplate);
            mainPageVM.SelectedVars = mainPageVM.VarsList.FirstOrDefault();
            mainPageVM.VarsVisible = mainPageVM.SelectedVars != null;
            mainPageVM.EmptyFolders = generatorAdapter.TemplatesConfiguration.HasToRemoveFolders(mainPageVM.SelectedTemplate);
            mainPageVM.OutputPath = generatorAdapter.OutputPath;
            mainPageVM.SlnFile = generatorAdapter.SlnFile;

            mainPageVM.Working = false;
            CommandManager.InvalidateRequerySuggested();
        }

        private void ModuleChanged(MainPageVM mainPageVM)
        {
            mainPageVM.Working = true;
            generatorAdapter.SetCurrentModule(mainPageVM.SelectedModule);
            mainPageVM.OutputPath = generatorAdapter.OutputPath;
            mainPageVM.SlnFile = generatorAdapter.SlnFile;
            mainPageVM.AppType = generatorAdapter.AppType;

            mainPageVM.Working = false;
            CommandManager.InvalidateRequerySuggested();
        }

        private void VarsChanged(MainPageVM mainWindowVM)
        {
            generatorAdapter.SetCurrentVars(mainWindowVM.SelectedVars);
            mainWindowVM.OutputPath = generatorAdapter.OutputPath;
            mainWindowVM.SlnFile = generatorAdapter.SlnFile;
            mainWindowVM.AppType = generatorAdapter.AppType;
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
