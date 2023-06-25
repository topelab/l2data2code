using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Main.Interfaces;
using L2Data2CodeUI.Shared.Adapters;
using System;
using System.ComponentModel;
using System.Linq;

namespace L2Data2Code.SharedContext.Main
{
    internal class MainWindowVMChangeListener : IMainWindowVMChangeListener
    {
        private readonly IGeneratorAdapter generatorAdapter;
        private readonly ICommandManager commandManager;

        public MainWindowVMChangeListener(IGeneratorAdapter generatorAdapter, ICommandManager commandManager)
        {
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
            this.commandManager = commandManager ?? throw new ArgumentNullException(nameof(commandManager));
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
                    commandManager.InvalidateRequerySuggested();
                    break;
                case nameof(MainWindowVM.PauseTimer):
                    break;
                case nameof(MainWindowVM.RunningGenerateCode):
                    mainWindowVM.OnPropertyChanged(nameof(mainWindowVM.GenerateCodeCommand));
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
                    mainWindowVM.OnPropertyChanged(nameof(mainWindowVM.GenerateCodeCommand));
                    break;
                case nameof(MainWindowVM.TemplateList):
                    break;
                case nameof(MainWindowVM.VarsList):
                    break;
                case nameof(MainWindowVM.VarsVisible):
                    break;
                case nameof(MainWindowVM.GenerateCodeCommand):
                    commandManager.InvalidateRequerySuggested();
                    break;
                case nameof(MainWindowVM.Working):
                    if (!mainWindowVM.Working)
                    {
                        commandManager.InvalidateRequerySuggested();
                    }
                    break;
                default:
                    break;
            }
        }

        private void AreaChanged(MainWindowVM mainWindowVM)
        {
            mainWindowVM.WorkOnAction(() =>
            {
                mainWindowVM.TablePanelVM.LoadingTables = true;
                mainWindowVM.TablePanelVM.ViewsVisible = false;
                var initialGenerateOnlyJsonVisible = bool.TryParse(generatorAdapter.SettingsConfiguration["generateJsonInfo"], out var generateJsonInfo) && generateJsonInfo;

                mainWindowVM.PauseTimer = true;
                generatorAdapter.SetCurrentDataSource(mainWindowVM.SelectedDataSource);
                mainWindowVM.TablePanelVM.LoadTablesCommand.Execute(null);
                mainWindowVM.ModuleList = generatorAdapter.GetModuleList(mainWindowVM.SelectedDataSource);
                mainWindowVM.SelectedModule = generatorAdapter.GetDefaultModule(mainWindowVM.SelectedDataSource);
                mainWindowVM.VarsList = generatorAdapter.GetVarsList(mainWindowVM.SelectedTemplate, mainWindowVM.SelectedDataSource);
                mainWindowVM.SelectedVars = mainWindowVM.VarsList.FirstOrDefault();

                mainWindowVM.OutputPath = generatorAdapter.OutputPath;
                mainWindowVM.SlnFile = generatorAdapter.SlnFile;
                mainWindowVM.GenerateOnlyJsonVisible = initialGenerateOnlyJsonVisible && generatorAdapter.InputSourceType != "json" && generatorAdapter.InputSourceType != "fake";
                mainWindowVM.TablePanelVM.LoadingTables = false;
                mainWindowVM.PauseTimer = false;
            });
        }

        private void TemplateChanged(MainWindowVM mainWindowVM)
        {
            mainWindowVM.WorkOnAction(() =>
            {
                generatorAdapter.SetCurrentTemplate(mainWindowVM.SelectedTemplate);
                mainWindowVM.VarsList = generatorAdapter.GetVarsList(mainWindowVM.SelectedTemplate);
                mainWindowVM.SelectedVars = mainWindowVM.VarsList.FirstOrDefault();
                mainWindowVM.VarsVisible = mainWindowVM.SelectedVars != null;
                mainWindowVM.EmptyFolders = generatorAdapter.TemplatesConfiguration.HasToRemoveFolders(mainWindowVM.SelectedTemplate);
                mainWindowVM.OutputPath = generatorAdapter.OutputPath;
                mainWindowVM.SlnFile = generatorAdapter.SlnFile;
            });
        }

        private void ModuleChanged(MainWindowVM mainWindowVM)
        {
            mainWindowVM.WorkOnAction(() =>
            {
                generatorAdapter.SetCurrentModule(mainWindowVM.SelectedModule);
                mainWindowVM.OutputPath = generatorAdapter.OutputPath;
                mainWindowVM.SlnFile = generatorAdapter.SlnFile;
                mainWindowVM.AppType = generatorAdapter.AppType;
            });
        }

        private void VarsChanged(MainWindowVM mainWindowVM)
        {
            mainWindowVM.WorkOnAction(() =>
            {
                generatorAdapter.SetCurrentVars(mainWindowVM.SelectedVars);
                mainWindowVM.OutputPath = generatorAdapter.OutputPath;
                mainWindowVM.SlnFile = generatorAdapter.SlnFile;
                mainWindowVM.AppType = generatorAdapter.AppType;
            });
        }
    }
}
