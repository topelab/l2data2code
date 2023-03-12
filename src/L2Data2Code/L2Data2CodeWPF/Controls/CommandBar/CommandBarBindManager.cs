using L2Data2CodeUI.Shared.Dto;
using L2Data2CodeWPF.Main;
using System.ComponentModel;

namespace L2Data2CodeWPF.Controls.CommandBar
{
    internal class CommandBarBindManager : ICommandBarBindManager
    {
        private MainWindowVM mainVM;
        private CommandBarVM controlVM;

        public void Start(MainWindowVM mainVM, CommandBarVM controlVM)
        {
            Stop();
            this.mainVM = mainVM;
            this.controlVM = controlVM;
            mainVM.PropertyChanged += OnParentVMPropertyChanged;
        }

        public void Stop()
        {
            if (mainVM != null)
            {
                mainVM.PropertyChanged -= OnParentVMPropertyChanged;
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
    }
}
