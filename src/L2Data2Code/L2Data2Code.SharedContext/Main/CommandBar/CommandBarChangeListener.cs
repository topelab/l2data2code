using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Events;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Dto;
using L2Data2CodeUI.Shared.Localize;
using Prism.Events;
using System.ComponentModel;
using System.IO;

namespace L2Data2Code.SharedContext.Main.CommandBar
{
    public class CommandBarChangeListener : ICommandBarChangeListener
    {
        private MainWindowVM mainVM;
        private CommandBarVM controlVM;

        private readonly IDispatcherWrapper dispatcher;
        private readonly IFileMonitorService fileMonitorService;
        private readonly IEventAggregator eventAggregator;

        public CommandBarChangeListener(IDispatcherWrapper dispatcherWrapper, IFileMonitorService fileMonitorService, IEventAggregator eventAggregator)
        {
            dispatcher = dispatcherWrapper ?? throw new System.ArgumentNullException(nameof(dispatcherWrapper));
            this.fileMonitorService = fileMonitorService ?? throw new System.ArgumentNullException(nameof(fileMonitorService));
            this.eventAggregator = eventAggregator ?? throw new System.ArgumentNullException(nameof(eventAggregator));
        }

        public void Start(MainWindowVM mainVM, CommandBarVM controlVM)
        {
            Stop();
            this.mainVM = mainVM;
            this.controlVM = controlVM;
            mainVM.PropertyChanged += OnParentVMPropertyChanged;
            controlVM.PropertyChanged += OnControlVMPropertyChanged;
        }

        public void Stop()
        {
            if (mainVM != null)
            {
                mainVM.PropertyChanged -= OnParentVMPropertyChanged;
            }

            if (controlVM != null)
            {
                controlVM.PropertyChanged -= OnControlVMPropertyChanged;
            }

            fileMonitorService.StopMonitoring();
        }

        private void OnParentVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MainWindowVM.Working):
                    controlVM.Working = mainVM.Working;
                    break;
                case nameof(MainWindowVM.ShowSettingsWindow):
                    controlVM.ShowVarsWindow = mainVM.ShowSettingsWindow;
                    break;
                case nameof(MainWindowVM.HaveVSCodeInstalled):
                    controlVM.HaveVSCodeInstalled = mainVM.HaveVSCodeInstalled;
                    break;
                case nameof(MainWindowVM.HavePSInstalled):
                    controlVM.HavePSInstalled = mainVM.HavePSInstalled;
                    break;
                case nameof(MainWindowVM.VSCodePath):
                    controlVM.VSCodePath = mainVM.VSCodePath;
                    break;
                case nameof(MainWindowVM.SlnFile):
                    controlVM.SlnFile = mainVM.SlnFile;
                    controlVM.OnPropertyChanged(nameof(controlVM.ChangeButtons));
                    controlVM.OnPropertyChanged(nameof(controlVM.OpenVSCommand));
                    controlVM.OnPropertyChanged(nameof(controlVM.OpenFolderCommand));
                    controlVM.OnPropertyChanged(nameof(controlVM.OpenPSCommand));
                    controlVM.OnPropertyChanged(nameof(controlVM.OpenVSCodeCommand));
                    break;
                case nameof(MainWindowVM.AppType):
                    controlVM.CanShowVSButton = mainVM.AppType == AppType.VisualStudio;
                    break;
                case nameof(MainWindowVM.OutputPath):
                    if (mainVM.OutputPath != null)
                    {
                        WatchOutputPath();
                    }
                    break;
                case nameof(MainWindowVM.RunApplication):
                    controlVM.RunAction = mainVM.RunApplication;
                    break;
                case nameof(MainWindowVM.RunningGenerateCode):
                    if (!mainVM.RunningGenerateCode)
                    {
                        WatchOutputPath();
                    }
                    break;
                default:
                    break;
            }
        }

        private void WatchOutputPath()
        {
            var parent = Path.GetDirectoryName(mainVM.OutputPath.TrimPathSeparator());
            var last = mainVM.OutputPath.Replace(parent, "").Trim('\\');
            var action = () =>
            {
                dispatcher.Invoke(() => { mainVM.CheckButtonStates(); });
            };
            fileMonitorService.StartMonitoring(file => action(), parent, last);
        }

        private void OnControlVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CommandBarVM.ChangeButtons):
                    dispatcher.Invoke(() =>
                    {
                        eventAggregator.GetEvent<SelectAppTypeEvent>().Publish(mainVM.AppType);

                        controlVM.OpenCmdToolTip = mainVM.AppType switch
                        {
                            AppType.VisualStudio => Strings.OpenVSSolution,
                            AppType.VisualStudioCode => Strings.OpenVSC,
                            AppType.ApacheNetBeans => Strings.Open + " Apache NetBeans",
                            AppType.Eclipse => Strings.Open + " Eclipse",
                            AppType.IntelliJIdea => Strings.Open + " IntelliJIdea",
                            _ => string.Empty
                        };

                    });
                    break;
                default:
                    break;
            }
        }

    }
}
