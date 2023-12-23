using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Dto;
using System;
using System.Linq;
using L2Data2Code.SharedContext.Main.MessagePanel;
using L2Data2Code.SharedContext.Main.CommandBar;
using L2Data2Code.SharedContext.Main.Interfaces;
using L2Data2Code.SharedContext.Main.TablePanel;
using NLog;

namespace L2Data2Code.SharedContext.Main
{
    public class MainWindowVMInitializer : IMainWindowVMInitializer
    {
        private readonly IGeneratorAdapter generatorAdapter;
        private readonly ICommandBarFactory commandBarFactory;
        private readonly ITablePanelFactory tablePanelFactory;
        private readonly IMessagePanelFactory messagePanelFactory;
        private readonly IProcessManager processManager;
        private readonly ILogger logger;

        public MainWindowVMInitializer(IGeneratorAdapter generatorAdapter, ICommandBarFactory commandBarFactory,
                            ITablePanelFactory tablePanelFactory,
                            IMessagePanelFactory messagePanelFactory, IProcessManager processManager, ILogger logger)
        {
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
            this.commandBarFactory = commandBarFactory ?? throw new ArgumentNullException(nameof(commandBarFactory));
            this.tablePanelFactory = tablePanelFactory ?? throw new ArgumentNullException(nameof(tablePanelFactory));
            this.messagePanelFactory = messagePanelFactory ?? throw new ArgumentNullException(nameof(messagePanelFactory));
            this.processManager = processManager ?? throw new ArgumentNullException(nameof(processManager));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialize(MainWindowVM mainWindowVM)
        {
            logger.Info($"Opening {nameof(MainWindowVM)}");

            mainWindowVM.CommandBarVM = commandBarFactory.Create(mainWindowVM);
            mainWindowVM.TablePanelVM = tablePanelFactory.Create(mainWindowVM);
            mainWindowVM.MessagePanelVM = messagePanelFactory.Create(mainWindowVM);

            mainWindowVM.VSCodePath = processManager.FindVSCode();
            mainWindowVM.HaveVSCodeInstalled = mainWindowVM.VSCodePath.NotEmpty();

            mainWindowVM.PSPath = processManager.FindPS();
            mainWindowVM.HavePSInstalled = mainWindowVM.PSPath.NotEmpty();

            mainWindowVM.CommandBarVM.CanShowVSButton = mainWindowVM.AppType == AppType.VisualStudio;

            mainWindowVM.ShowSettingsWindow = bool.TryParse(generatorAdapter.SettingsConfiguration["showVarsWindow"], out var showVarsWindow) && showVarsWindow;

            mainWindowVM.EmptyFolders = true;
            mainWindowVM.SetRelatedTables = true;

            generatorAdapter.GetTemplateList().ToList().ForEach(t => mainWindowVM.TemplateList.Add(t));
            generatorAdapter.GetAreaList().ToList().ForEach(t => mainWindowVM.DataSourceList.Add(t));

            var selectedTemplate = mainWindowVM.TemplateList.FirstOrDefault();
            var selectedDataSource = mainWindowVM.DataSourceList.FirstOrDefault();

            mainWindowVM.SelectedTemplate = selectedTemplate;
            mainWindowVM.SelectedDataSource = selectedDataSource;
            mainWindowVM.ModuleList.Clear();
            generatorAdapter.GetModuleList(selectedDataSource).ToList().ForEach(t => mainWindowVM.ModuleList.Add(t));
            mainWindowVM.SelectedModule = generatorAdapter.GetDefaultModule(selectedDataSource);
            mainWindowVM.Settings.Clear();
            generatorAdapter.GetSettings(selectedTemplate, selectedDataSource).ToList().ForEach(t => mainWindowVM.Settings.Add(t));
            mainWindowVM.SelectedSetting = mainWindowVM.Settings.FirstOrDefault();

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
