using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Dto;
using L2Data2Code.Main.CommandBar;
using L2Data2Code.Main.MessagePanel;
using L2Data2Code.Main.TablePanel;
using System;
using System.Linq;
using L2Data2Code.Main.Interfaces;

namespace L2Data2Code.Main
{
    internal class MainWindowVMInitializer : IMainWindowVMInitializer
    {
        private readonly IGeneratorAdapter generatorAdapter;
        private readonly ICommandBarFactory commandBarFactory;
        private readonly ITablePanelFactory tablePanelFactory;
        private readonly IMessagePanelFactory messagePanelFactory;
        private readonly IProcessManager processManager;

        public MainWindowVMInitializer(IGeneratorAdapter generatorAdapter, ICommandBarFactory commandBarFactory,
                            ITablePanelFactory tablePanelFactory,
                            IMessagePanelFactory messagePanelFactory, IProcessManager processManager)
        {
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
            this.commandBarFactory = commandBarFactory ?? throw new ArgumentNullException(nameof(commandBarFactory));
            this.tablePanelFactory = tablePanelFactory ?? throw new ArgumentNullException(nameof(tablePanelFactory));
            this.messagePanelFactory = messagePanelFactory ?? throw new ArgumentNullException(nameof(messagePanelFactory));
            this.processManager = processManager ?? throw new ArgumentNullException(nameof(processManager));
        }

        public void Initialize(MainWindowVM mainWindowVM)
        {
            App.Logger.Info($"Opening {nameof(MainWindowVM)}");

            mainWindowVM.CommandBarVM = commandBarFactory.Create(mainWindowVM);
            mainWindowVM.TablePanelVM = tablePanelFactory.Create(mainWindowVM);
            mainWindowVM.MessagePanelVM = messagePanelFactory.Create(mainWindowVM);

            mainWindowVM.VSCodePath = processManager.FindVSCode();
            mainWindowVM.HaveVSCodeInstalled = mainWindowVM.VSCodePath.NotEmpty();

            mainWindowVM.PSPath = processManager.FindPS();
            mainWindowVM.HavePSInstalled = mainWindowVM.PSPath.NotEmpty();

            mainWindowVM.CommandBarVM.CanShowVSButton = mainWindowVM.AppType == AppType.VisualStudio;

            mainWindowVM.ShowVarsWindow = bool.TryParse(generatorAdapter.SettingsConfiguration["showVarsWindow"], out var showVarsWindow) && showVarsWindow;

            mainWindowVM.EmptyFolders = true;
            mainWindowVM.SetRelatedTables = true;

            mainWindowVM.TemplateList = generatorAdapter.GetTemplateList();
            mainWindowVM.DataSourceList = generatorAdapter.GetAreaList();

            var selectedTemplate = mainWindowVM.TemplateList.FirstOrDefault();
            var selectedDataSource = mainWindowVM.DataSourceList.FirstOrDefault();

            mainWindowVM.SelectedTemplate = selectedTemplate;
            mainWindowVM.SelectedDataSource = selectedDataSource;
            mainWindowVM.ModuleList = generatorAdapter.GetModuleList(selectedDataSource);
            mainWindowVM.SelectedModule = generatorAdapter.GetDefaultModule(selectedDataSource);
            mainWindowVM.VarsList = generatorAdapter.GetVarsList(selectedTemplate, selectedDataSource);
            mainWindowVM.SelectedVars = mainWindowVM.VarsList.FirstOrDefault();

            mainWindowVM.TablePanelVM.SelectAllTables = false;
            mainWindowVM.TablePanelVM.SelectAllViews = false;

            generatorAdapter.OnConfigurationChanged = () =>
            {
                mainWindowVM.OnPropertyChanged(nameof(mainWindowVM.SelectedTemplate));
                mainWindowVM.OnPropertyChanged(nameof(mainWindowVM.SelectedDataSource));
            };

        }
    }
}
