using L2Data2Code.SharedContext.Base;
using L2Data2CodeUI.Shared.Localize;
using System.Windows.Input;

namespace L2Data2Code.SharedContext.Main.CommandBar
{
    public class CommandBarVM : ViewModelBase
    {
        private ICommand openFolderCommand;
        private ICommand editTemplateCommand;
        private ICommand openSettingsCommand;
        private ICommand openVSCommand;
        private ICommand openVSCodeCommand;
        private ICommand openPSCommand;
        private ICommand openVarsWindowCommand;
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

        public void SetCommands(ICommand openFolderCommand,
                                ICommand editTemplateCommand,
                                ICommand openSettingsCommand,
                                ICommand openVSCommand,
                                ICommand openVSCodeCommand,
                                ICommand openPSCommand,
                                ICommand openVarsWindowCommand)
        {
            this.openFolderCommand = openFolderCommand;
            this.editTemplateCommand = editTemplateCommand;
            this.openSettingsCommand = openSettingsCommand;
            this.openVSCommand = openVSCommand;
            this.openVSCodeCommand = openVSCodeCommand;
            this.openPSCommand = openPSCommand;
            this.openVarsWindowCommand = openVarsWindowCommand;
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
        public ICommand OpenFolderCommand => openFolderCommand;
        /// <summary>
        /// Gets the edit template command.
        /// </summary>
        /// <value>
        /// The edit template command.
        /// </value>
        public ICommand EditTemplateCommand => editTemplateCommand;
        /// <summary>
        /// Gets the open settings command.
        /// </summary>
        /// <value>
        /// The open settings command.
        /// </value>
        public ICommand OpenSettingsCommand => openSettingsCommand;
        /// <summary>
        /// Gets the open vs command.
        /// </summary>
        /// <value>
        /// The open vs command.
        /// </value>
        public ICommand OpenVSCommand => openVSCommand;
        /// <summary>
        /// Gets the open vs command.
        /// </summary>
        /// <value>
        /// The open vs command.
        /// </value>
        public ICommand OpenVSCodeCommand => openVSCodeCommand;
        /// <summary>
        /// Gets the open ps command.
        /// </summary>
        /// <value>
        /// The open ps command.
        /// </value>
        public ICommand OpenPSCommand => openPSCommand;
        /// <summary>
        /// Gets the open vars window.
        /// </summary>
        /// <value>
        /// The open vars window.
        /// </value>
        public ICommand OpenVarsWindowCommand => openVarsWindowCommand;
    }
}