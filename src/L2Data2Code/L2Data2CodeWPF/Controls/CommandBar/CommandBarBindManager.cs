using L2Data2CodeUI.Shared.Dto;
using L2Data2CodeUI.Shared.Localize;
using L2Data2CodeWPF.Main;
using L2Data2CodeWPF.SharedLib;
using MahApps.Metro.IconPacks;
using System.ComponentModel;

namespace L2Data2CodeWPF.Controls.CommandBar
{
    internal class CommandBarBindManager : ICommandBarBindManager
    {
        private MainWindowVM mainVM;
        private CommandBarVM controlVM;

        private readonly IDispatcherWrapper dispatcher;

        public CommandBarBindManager(IDispatcherWrapper dispatcherWrapper)
        {
            this.dispatcher = dispatcherWrapper ?? throw new System.ArgumentNullException(nameof(dispatcherWrapper));
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
        }

        private void OnParentVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MainWindowVM.Working):
                    controlVM.Working = mainVM.Working;
                    break;
                case nameof(MainWindowVM.ShowVarsWindow):
                    controlVM.ShowVarsWindow = mainVM.ShowVarsWindow;
                    break;
                case nameof(MainWindowVM.HaveVSCodeInstalled):
                    controlVM.HaveVSCodeInstalled = mainVM.HaveVSCodeInstalled;
                    break;
                case nameof(MainWindowVM.HavePSInstalled):
                    controlVM.HavePSInstalled = mainVM.HavePSInstalled;
                    break;
                case (nameof(MainWindowVM.VSCodePath)):
                    controlVM.VSCodePath = mainVM.VSCodePath;
                    break;
                case (nameof(MainWindowVM.SlnFile)):
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
                default:
                    break;
            }
        }

        private void OnControlVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CommandBarVM.ChangeButtons):
                    dispatcher.Invoke(() =>
                    {
                        controlVM.OpenCmdIcon.Kind = mainVM.AppType switch
                        {
                            AppType.VisualStudio => PackIconSimpleIconsKind.VisualStudio,
                            AppType.VisualStudioCode => PackIconSimpleIconsKind.VisualStudioCode,
                            AppType.ApacheNetBeans => PackIconSimpleIconsKind.ApacheNetBeansIde,
                            AppType.Eclipse => PackIconSimpleIconsKind.EclipseIde,
                            AppType.IntelliJIdea => PackIconSimpleIconsKind.IntelliJIdea,
                            _ => PackIconSimpleIconsKind.None
                        };
                        controlVM.OnPropertyChanged(nameof(controlVM.OpenCmdIcon));

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
