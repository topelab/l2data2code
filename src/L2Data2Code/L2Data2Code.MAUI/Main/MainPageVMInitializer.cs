using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Dto;
using L2Data2Code.MAUI.Main.CommandBar;
using L2Data2Code.MAUI.Main.MessagePanel;
using L2Data2Code.MAUI.Main.TablePanel;
using System;
using System.Linq;
using L2Data2Code.MAUI.Main.Interfaces;

namespace L2Data2Code.MAUI.Main
{
    internal class MainPageVMInitializer : IMainPageVMInitializer
    {
        private readonly IGeneratorAdapter generatorAdapter;
        private readonly ICommandBarFactory commandBarFactory;
        private readonly ITablePanelFactory tablePanelFactory;
        private readonly IMessagePanelFactory messagePanelFactory;
        private readonly IProcessManager processManager;

        public MainPageVMInitializer(IGeneratorAdapter generatorAdapter, ICommandBarFactory commandBarFactory,
                            ITablePanelFactory tablePanelFactory,
                            IMessagePanelFactory messagePanelFactory, IProcessManager processManager)
        {
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
            this.commandBarFactory = commandBarFactory ?? throw new ArgumentNullException(nameof(commandBarFactory));
            this.tablePanelFactory = tablePanelFactory ?? throw new ArgumentNullException(nameof(tablePanelFactory));
            this.messagePanelFactory = messagePanelFactory ?? throw new ArgumentNullException(nameof(messagePanelFactory));
            this.processManager = processManager ?? throw new ArgumentNullException(nameof(processManager));
        }

        public void Initialize(MainPageVM mainPageVM)
        {
            App.Logger.Info($"Opening {nameof(MainPageVM)}");

            mainPageVM.CommandBarVM = commandBarFactory.Create(mainPageVM);
            mainPageVM.TablePanelVM = tablePanelFactory.Create(mainPageVM);
            mainPageVM.MessagePanelVM = messagePanelFactory.Create(mainPageVM);

            mainPageVM.VSCodePath = processManager.FindVSCode();
            mainPageVM.HaveVSCodeInstalled = mainPageVM.VSCodePath.NotEmpty();

            mainPageVM.PSPath = processManager.FindPS();
            mainPageVM.HavePSInstalled = mainPageVM.PSPath.NotEmpty();

            mainPageVM.CommandBarVM.CanShowVSButton = mainPageVM.AppType == AppType.VisualStudio;

            mainPageVM.ShowVarsWindow = bool.TryParse(generatorAdapter.SettingsConfiguration["showVarsWindow"], out var showVarsWindow) && showVarsWindow;

            mainPageVM.EmptyFolders = true;
            mainPageVM.SetRelatedTables = true;

            mainPageVM.TemplateList = generatorAdapter.GetTemplateList();
            mainPageVM.DataSourceList = generatorAdapter.GetAreaList();

            var selectedTemplate = mainPageVM.TemplateList.FirstOrDefault();
            var selectedDataSource = mainPageVM.DataSourceList.FirstOrDefault();

            mainPageVM.SelectedTemplate = selectedTemplate;
            mainPageVM.SelectedDataSource = selectedDataSource;
            mainPageVM.ModuleList = generatorAdapter.GetModuleList(selectedDataSource);
            mainPageVM.SelectedModule = generatorAdapter.GetDefaultModule(selectedDataSource);
            mainPageVM.VarsList = generatorAdapter.GetVarsList(selectedTemplate, selectedDataSource);
            mainPageVM.SelectedVars = mainPageVM.VarsList.FirstOrDefault();

            mainPageVM.TablePanelVM.SelectAllTables = false;
            mainPageVM.TablePanelVM.SelectAllViews = false;

            generatorAdapter.OnConfigurationChanged = () =>
            {
                mainPageVM.OnPropertyChanged(nameof(mainPageVM.SelectedTemplate));
                mainPageVM.OnPropertyChanged(nameof(mainPageVM.SelectedDataSource));
            };

        }
    }
}
