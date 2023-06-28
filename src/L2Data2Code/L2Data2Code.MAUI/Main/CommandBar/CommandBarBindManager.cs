using L2Data2Code.MAUI.Base;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Dto;
using L2Data2CodeUI.Shared.Localize;
//using Material.Icons;
using System.ComponentModel;
using System.IO;

namespace L2Data2Code.MAUI.Main.CommandBar
{
    internal class CommandBarBindManager : ICommandBarBindManager
    {
        private MainPageVM mainVM;
        private CommandBarVM controlVM;

        private readonly IDispatcherWrapper dispatcher;
        private readonly IFileMonitorService fileMonitorService;

        public CommandBarBindManager(IDispatcherWrapper dispatcherWrapper, IFileMonitorService fileMonitorService)
        {
            this.dispatcher = dispatcherWrapper ?? throw new System.ArgumentNullException(nameof(dispatcherWrapper));
            this.fileMonitorService = fileMonitorService ?? throw new System.ArgumentNullException(nameof(fileMonitorService));
        }

        public void Start(MainPageVM mainVM, CommandBarVM controlVM)
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
                case nameof(MainPageVM.Working):
                    controlVM.Working = mainVM.Working;
                    break;
                case nameof(MainPageVM.ShowVarsWindow):
                    controlVM.ShowVarsWindow = mainVM.ShowVarsWindow;
                    break;
                case nameof(MainPageVM.HaveVSCodeInstalled):
                    controlVM.HaveVSCodeInstalled = mainVM.HaveVSCodeInstalled;
                    break;
                case nameof(MainPageVM.HavePSInstalled):
                    controlVM.HavePSInstalled = mainVM.HavePSInstalled;
                    break;
                case (nameof(MainPageVM.VSCodePath)):
                    controlVM.VSCodePath = mainVM.VSCodePath;
                    break;
                case (nameof(MainPageVM.SlnFile)):
                    controlVM.SlnFile = mainVM.SlnFile;
                    controlVM.OnPropertyChanged(nameof(controlVM.ChangeButtons));
                    controlVM.OnPropertyChanged(nameof(controlVM.OpenVSCommand));
                    controlVM.OnPropertyChanged(nameof(controlVM.OpenFolderCommand));
                    controlVM.OnPropertyChanged(nameof(controlVM.OpenPSCommand));
                    controlVM.OnPropertyChanged(nameof(controlVM.OpenVSCodeCommand));
                    break;
                case nameof(MainPageVM.AppType):
                    controlVM.CanShowVSButton = mainVM.AppType == AppType.VisualStudio;
                    break;
                case nameof(MainPageVM.OutputPath):
                    if (mainVM.OutputPath != null)
                    {
                        WatchOutputPath();
                    }
                    break;
                case nameof(MainPageVM.RunningGenerateCode):
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
                        //controlVM.OpenCmdIcon.Kind = mainVM.AppType switch
                        //{
                        //    AppType.VisualStudio => MaterialIconKind.MicrosoftVisualStudio,
                        //    AppType.VisualStudioCode => MaterialIconKind.MicrosoftVisualStudioCode,
                        //    AppType.ApacheNetBeans => MaterialIconKind.Application,
                        //    AppType.Eclipse => MaterialIconKind.Application,
                        //    AppType.IntelliJIdea => MaterialIconKind.Application,
                        //    _ => MaterialIconKind.Application
                        //};
                        //controlVM.OnPropertyChanged(nameof(controlVM.OpenCmdIcon));

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