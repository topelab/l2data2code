using L2Data2CodeUI.Shared.Adapters;
using L2Data2Code.Base;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using L2Data2Code.Main.Interfaces;

namespace L2Data2Code.Main
{
    internal class MainWindowVMBindManager : IMainWindowVMBindManager
    {
        private readonly IGeneratorAdapter generatorAdapter;
        private readonly IDispatcherWrapper dispatcher;

        public MainWindowVMBindManager(IGeneratorAdapter generatorAdapter, IDispatcherWrapper dispatcher)
        {
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public void Start(MainWindowVM mainWindowVM)
        {
            mainWindowVM.PropertyChanged += OnMainWindowVMPropertyChanged;
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        private void OnMainWindowVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var mainWindowVM = sender as MainWindowVM;
            switch (e.PropertyName)
            {
                case nameof(MainWindowVM.EmptyFolders):
                    break;
                case nameof(MainWindowVM.GenerateOnlyJson):
                    break;
                case nameof(MainWindowVM.GenerateOnlyJsonVisible):
                    break;
                case nameof(MainWindowVM.HavePSInstalled):
                    break;
                case nameof(MainWindowVM.HaveVSCodeInstalled):
                    break;
                case nameof(MainWindowVM.ModuleList):
                    break;
                case nameof(MainWindowVM.OutputPath):
                    break;
                case nameof(MainWindowVM.PauseTimer):
                    break;
                case nameof(MainWindowVM.RunningGenerateCode):
                    break;
                case nameof(MainWindowVM.SelectedDataSource):
                    AreaChanged(mainWindowVM);
                    break;
                case nameof(MainWindowVM.SelectedModule):
                    ModuleChanged(mainWindowVM);
                    break;
                case nameof(MainWindowVM.SelectedTemplate):
                    TemplateChanged(mainWindowVM);
                    break;
                case nameof(MainWindowVM.SelectedVars):
                    VarsChanged(mainWindowVM);
                    break;
                case nameof(MainWindowVM.SetRelatedTables):
                    break;
                case nameof(MainWindowVM.ShowVarsWindow):
                    break;
                case nameof(MainWindowVM.SlnFile):
                    break;
                case nameof(MainWindowVM.TemplateList):
                    break;
                case nameof(MainWindowVM.VarsList):
                    break;
                case nameof(MainWindowVM.VarsVisible):
                    break;
                default:
                    break;
            }
        }

        private void AreaChanged(MainWindowVM mainWindowVM)
        {
            mainWindowVM.Working = true;
            mainWindowVM.TablePanelVM.LoadingTables = true;
            mainWindowVM.TablePanelVM.ViewsVisible = false;
            var initialGenerateOnlyJsonVisible = bool.TryParse(generatorAdapter.SettingsConfiguration["generateJsonInfo"], out var generateJsonInfo) && generateJsonInfo;

            Task.Run(() =>
            {
                mainWindowVM.PauseTimer = true;
                generatorAdapter.SetCurrentDataSource(mainWindowVM.SelectedDataSource);
                mainWindowVM.TablePanelVM.LoadTablesCommand.Execute(null);
                dispatcher?.Invoke(() =>
                {
                    mainWindowVM.SelectedModule = generatorAdapter.SelectedModule;
                    mainWindowVM.ModuleList = generatorAdapter.GetModuleList(mainWindowVM.SelectedDataSource);
                    mainWindowVM.VarsList = generatorAdapter.GetVarsList(mainWindowVM.SelectedTemplate, mainWindowVM.SelectedDataSource);
                    mainWindowVM.SelectedVars = mainWindowVM.VarsList.FirstOrDefault();
                    mainWindowVM.OutputPath = generatorAdapter.OutputPath;
                    mainWindowVM.SlnFile = generatorAdapter.SlnFile;
                    mainWindowVM.GenerateOnlyJsonVisible = initialGenerateOnlyJsonVisible && generatorAdapter.InputSourceType != "json" && generatorAdapter.InputSourceType != "fake";
                });
            }).ContinueWith((t) =>
            {
                dispatcher?.Invoke(() =>
                {
                    mainWindowVM.TablePanelVM.LoadingTables = false;
                    mainWindowVM.Working = false;
                    mainWindowVM.PauseTimer = false;
                });
            });
        }

        private void TemplateChanged(MainWindowVM mainWindowVM)
        {
            mainWindowVM.Working = true;
            generatorAdapter.SetCurrentTemplate(mainWindowVM.SelectedTemplate);
            mainWindowVM.VarsList = generatorAdapter.GetVarsList(mainWindowVM.SelectedTemplate);
            mainWindowVM.SelectedVars = mainWindowVM.VarsList.FirstOrDefault();
            mainWindowVM.VarsVisible = mainWindowVM.SelectedVars != null;
            mainWindowVM.EmptyFolders = generatorAdapter.TemplatesConfiguration.HasToRemoveFolders(mainWindowVM.SelectedTemplate);
            mainWindowVM.OutputPath = generatorAdapter.OutputPath;
            mainWindowVM.SlnFile = generatorAdapter.SlnFile;

            mainWindowVM.Working = false;
        }

        private void ModuleChanged(MainWindowVM mainWindowVM)
        {
            mainWindowVM.Working = true;
            generatorAdapter.SetCurrentModule(mainWindowVM.SelectedModule);
            mainWindowVM.OutputPath = generatorAdapter.OutputPath;
            mainWindowVM.SlnFile = generatorAdapter.SlnFile;
            mainWindowVM.AppType = generatorAdapter.AppType;

            mainWindowVM.Working = false;
        }

        private void VarsChanged(MainWindowVM mainWindowVM)
        {
            generatorAdapter.SetCurrentVars(mainWindowVM.SelectedVars);
            mainWindowVM.OutputPath = generatorAdapter.OutputPath;
            mainWindowVM.SlnFile = generatorAdapter.SlnFile;
            mainWindowVM.AppType = generatorAdapter.AppType;
        }
    }
}
