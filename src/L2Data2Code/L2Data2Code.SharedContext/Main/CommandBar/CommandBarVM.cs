using L2Data2Code.SharedContext.Base;
using L2Data2CodeUI.Shared.Localize;

namespace L2Data2Code.SharedContext.Main.CommandBar
{
    public class CommandBarVM : ViewModelBase
    {
        private IDelegateCommand openFolderCommand;
        private IDelegateCommand editTemplateCommand;
        private IDelegateCommand openSettingsCommand;
        private IDelegateCommand openVSCommand;
        private IDelegateCommand openVSCodeCommand;
        private IDelegateCommand openPSCommand;
        private IDelegateCommand openVarsWindowCommand;
        private bool changeButtons;
        private bool showVarsWindow;
        private bool haveVSCodeInstalled;
        private bool havePSInstalled;
        private string vSCodePath;
        private string slnFile;
        private bool canShowVSButton;
        private string openCmdToolTip;

        public CommandBarVM()
        {
            openCmdToolTip = Strings.OpenVSSolution;
        }

        public void SetCommands(IDelegateCommand openFolderCommand,
                                IDelegateCommand editTemplateCommand,
                                IDelegateCommand openSettingsCommand,
                                IDelegateCommand openVSCommand,
                                IDelegateCommand openVSCodeCommand,
                                IDelegateCommand openPSCommand,
                                IDelegateCommand openVarsWindowCommand)
        {
            this.openFolderCommand = openFolderCommand;
            this.editTemplateCommand = editTemplateCommand;
            this.openSettingsCommand = openSettingsCommand;
            this.openVSCommand = openVSCommand;
            this.openVSCodeCommand = openVSCodeCommand;
            this.openPSCommand = openPSCommand;
            this.openVarsWindowCommand = openVarsWindowCommand;
        }

        internal void RefreshCommands()
        {
            openFolderCommand.RaiseCanExecuteChanged();
            editTemplateCommand.RaiseCanExecuteChanged();
            openSettingsCommand.RaiseCanExecuteChanged();
            openVSCommand.RaiseCanExecuteChanged();
            openVSCodeCommand.RaiseCanExecuteChanged();
            openPSCommand.RaiseCanExecuteChanged();
            openVarsWindowCommand.RaiseCanExecuteChanged();
        }

        public bool ChangeButtons { get => changeButtons; internal set => SetProperty(ref changeButtons, value); }
        public bool ShowVarsWindow { get => showVarsWindow; internal set => SetProperty(ref showVarsWindow, value); }
        public bool HaveVSCodeInstalled { get => haveVSCodeInstalled; internal set => SetProperty(ref haveVSCodeInstalled, value); }
        public bool HavePSInstalled { get => havePSInstalled; internal set => SetProperty(ref havePSInstalled, value); }
        public string VSCodePath { get => vSCodePath; internal set => SetProperty(ref vSCodePath, value); }
        public string SlnFile { get => slnFile; internal set => SetProperty(ref slnFile, value); }
        public bool CanShowVSButton { get => canShowVSButton; internal set => SetProperty(ref canShowVSButton, value); }
        public string OpenCmdToolTip { get => openCmdToolTip; internal set => SetProperty(ref openCmdToolTip, value); }

        /// <summary>
        /// Gets the open folder command.
        /// </summary>
        /// <value>
        /// The open folder command.
        /// </value>
        public IDelegateCommand OpenFolderCommand => openFolderCommand;
        /// <summary>
        /// Gets the edit template command.
        /// </summary>
        /// <value>
        /// The edit template command.
        /// </value>
        public IDelegateCommand EditTemplateCommand => editTemplateCommand;
        /// <summary>
        /// Gets the open settings command.
        /// </summary>
        /// <value>
        /// The open settings command.
        /// </value>
        public IDelegateCommand OpenSettingsCommand => openSettingsCommand;
        /// <summary>
        /// Gets the open vs command.
        /// </summary>
        /// <value>
        /// The open vs command.
        /// </value>
        public IDelegateCommand OpenVSCommand => openVSCommand;
        /// <summary>
        /// Gets the open vs command.
        /// </summary>
        /// <value>
        /// The open vs command.
        /// </value>
        public IDelegateCommand OpenVSCodeCommand => openVSCodeCommand;
        /// <summary>
        /// Gets the open ps command.
        /// </summary>
        /// <value>
        /// The open ps command.
        /// </value>
        public IDelegateCommand OpenPSCommand => openPSCommand;
        /// <summary>
        /// Gets the open vars window.
        /// </summary>
        /// <value>
        /// The open vars window.
        /// </value>
        public IDelegateCommand OpenVarsWindowCommand => openVarsWindowCommand;
    }
}
