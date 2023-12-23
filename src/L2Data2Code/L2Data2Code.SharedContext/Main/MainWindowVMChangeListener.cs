using L2Data2Code.BaseGenerator.Configuration;
using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Main.Interfaces;
using L2Data2CodeUI.Shared.Adapters;
using System;
using System.ComponentModel;
using System.Linq;

namespace L2Data2Code.SharedContext.Main
{
    public class MainWindowVMChangeListener : IMainWindowVMChangeListener
    {
        private readonly IGeneratorAdapter generatorAdapter;

        public MainWindowVMChangeListener(IGeneratorAdapter generatorAdapter)
        {
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
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
                    RefreshCommands(mainWindowVM);
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
                case nameof(MainWindowVM.SelectedSetting):
                    VarsChanged(mainWindowVM);
                    break;
                case nameof(MainWindowVM.SetRelatedTables):
                    break;
                case nameof(MainWindowVM.ShowSettingsWindow):
                    break;
                case nameof(MainWindowVM.SlnFile):
                    mainWindowVM.OnPropertyChanged(nameof(mainWindowVM.GenerateCodeCommand));
                    break;
                case nameof(MainWindowVM.TemplateList):
                    break;
                case nameof(MainWindowVM.Settings):
                    break;
                case nameof(MainWindowVM.VarsVisible):
                    break;
                case nameof(MainWindowVM.GenerateCodeCommand):
                    RefreshCommands(mainWindowVM);
                    break;
                case nameof(MainWindowVM.Working):
                    if (!mainWindowVM.Working)
                    {
                        RefreshCommands(mainWindowVM);
                    }
                    break;
                default:
                    break;
            }
        }

        private void RefreshCommands(MainWindowVM mainWindowVM)
        {
            mainWindowVM.GenerateCodeCommand.RaiseCanExecuteChanged();
            mainWindowVM.CommandBarVM.RefreshCommands();

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

                mainWindowVM.ModuleList.Clear();
                generatorAdapter.GetModuleList(mainWindowVM.SelectedDataSource).ToList().ForEach(t => mainWindowVM.ModuleList.Add(t));
                mainWindowVM.SelectedModule = generatorAdapter.GetDefaultModule(mainWindowVM.SelectedDataSource);

                mainWindowVM.Settings.Clear();
                generatorAdapter.GetSettings(mainWindowVM.SelectedTemplate, mainWindowVM.SelectedDataSource).ToList().ForEach(t => mainWindowVM.Settings.Add(t));
                mainWindowVM.SelectedSetting = mainWindowVM.Settings.FirstOrDefault();

                mainWindowVM.EmptyFolders = generatorAdapter.TemplatesConfiguration.HasToRemoveFolders(mainWindowVM.SelectedTemplate, generatorAdapter.DataSourcesConfiguration[mainWindowVM.SelectedDataSource].Vars?[nameof(TemplateConfiguration.RemoveFolders)]);

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

                mainWindowVM.Settings.Clear();
                generatorAdapter.GetSettings(mainWindowVM.SelectedTemplate).ToList().ForEach(t => mainWindowVM.Settings.Add(t));
                mainWindowVM.SelectedSetting = mainWindowVM.Settings.FirstOrDefault();

                mainWindowVM.VarsVisible = mainWindowVM.SelectedSetting != null;
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
                generatorAdapter.SetCurrentSetting(mainWindowVM.SelectedSetting);
                mainWindowVM.SelectedModule = generatorAdapter.SelectedModule;
                mainWindowVM.OutputPath = generatorAdapter.OutputPath;
                mainWindowVM.SlnFile = generatorAdapter.SlnFile;
                mainWindowVM.AppType = generatorAdapter.AppType;
            });
        }
    }
}
