using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Localize;
using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.ViewModel;
using System;
using System.ComponentModel;
using System.IO;

namespace L2Data2CodeWPF.Controls.CommandBar
{
    public class CommandBarViewModel : BaseControlViewModel
    {
        private readonly IGeneratorAdapter adapter;
        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly IMessageService messageService;
        private readonly IAppService appService;


        public CommandBarViewModel(IBaseViewModel baseViewModel) : base(baseViewModel)
        {
            mainWindowViewModel = (MainWindowViewModel)baseViewModel;
            adapter = mainWindowViewModel.Adapter;
            messageService = mainWindowViewModel.MessageService;
            appService = mainWindowViewModel.AppService;
        }

        public bool ChangeButtons { get; internal set; }

        public bool ShowVarsWindow
        {
            get { return mainWindowViewModel.ShowVarsWindow; }
            set { mainWindowViewModel.ShowVarsWindow = value; }
        }

        public bool HaveVSCodeInstalled => mainWindowViewModel.HaveVSCodeInstalled;
        public bool HavePSInstalled => mainWindowViewModel.HavePSInstalled;

        /// <summary>
        /// Gets the open folder command.
        /// </summary>
        /// <value>
        /// The open folder command.
        /// </value>
        public DelegateCommand OpenFolderCommand => new DelegateCommand(OnOpenFolderCommand, CanOpenFolderCommand);

        private bool CanOpenFolderCommand(object arg)
        {
            return Directory.Exists(Path.GetDirectoryName(mainWindowViewModel.SlnFile));
        }

        private void OnOpenFolderCommand(object obj)
        {
            Working = true;
            ProcessManager.Run(Path.GetDirectoryName(mainWindowViewModel.SlnFile));
            Working = false;
        }

        /// <summary>
        /// Gets the edit template command.
        /// </summary>
        /// <value>
        /// The edit template command.
        /// </value>
        public DelegateCommand EditTemplateCommand => new DelegateCommand(OnEditTemplateCommand, CanEditTemplateCommand);

        private bool CanEditTemplateCommand(object arg)
        {
            return CanEditTemplate();
        }

        private void OnEditTemplateCommand(object obj)
        {
            EditTemlate();
        }

        /// <summary>
        /// Gets the open settings command.
        /// </summary>
        /// <value>
        /// The open settings command.
        /// </value>
        public DelegateCommand OpenSettingsCommand => new DelegateCommand(OnOpenSettingsCommand, CanOpenSettingsCommand);

        private bool CanOpenSettingsCommand(object arg)
        {
            return CanOpenSettings();
        }

        private void OnOpenSettingsCommand(object obj)
        {
            OpenSettings();
        }

        /// <summary>
        /// Gets the open vs command.
        /// </summary>
        /// <value>
        /// The open vs command.
        /// </value>
        public DelegateCommand OpenVSCommand => new DelegateCommand(OnOpenVSCommand, CanOpenVSCommand);

        private bool CanOpenVSCommand(object arg)
        {
            return CheckCanOpenVS(mainWindowViewModel.OutputPath, out _);
        }

        private void OnOpenVSCommand(object obj)
        {
            if (CheckCanOpenVS(mainWindowViewModel.OutputPath, out string slnFile))
            {
                appService.Open(slnFile);
            }
        }

        /// <summary>
        /// Gets the open vs command.
        /// </summary>
        /// <value>
        /// The open vs command.
        /// </value>
        public DelegateCommand OpenVSCodeCommand => new DelegateCommand(OnOpenVSCodeCommand, CanOpenVSCodeCommand);

        private bool CanOpenVSCodeCommand(object arg)
        {
            return CheckCanOpenVS(mainWindowViewModel.OutputPath, out _) && mainWindowViewModel.HaveVSCodeInstalled;
        }

        private void OnOpenVSCodeCommand(object obj)
        {
            if (CheckCanOpenVS(mainWindowViewModel.OutputPath, out string slnFile))
            {
                appService.Open(slnFile, mainWindowViewModel.VSCodePath, AppService.DIRECTORY_PATTERN);
            }
        }

        /// <summary>
        /// Gets the open ps command.
        /// </summary>
        /// <value>
        /// The open ps command.
        /// </value>
        public DelegateCommand OpenPSCommand => new DelegateCommand(OnOpenPSCommand, CanOpenPSCommand);

        private bool CanOpenPSCommand(object arg)
        {
            return CheckCanOpenVS(mainWindowViewModel.OutputPath, out _);
        }

        private void OnOpenPSCommand(object obj)
        {
            if (CheckCanOpenVS(mainWindowViewModel.OutputPath, out string slnFile))
            {
                appService.Open(slnFile, mainWindowViewModel.PSPath, $"-noexit -command \"cd {AppService.DIRECTORY_PATTERN}\"");
            }
        }

        /// <summary>
        /// Gets the open vars window.
        /// </summary>
        /// <value>
        /// The open vars window.
        /// </value>
        public DelegateCommand OpenVarsWindow => new DelegateCommand(OnOpenVarsWindow, CanOpenVarsWindow);

        private bool CanOpenVarsWindow(object arg)
        {
            return true;
        }

        private void OnOpenVarsWindow(object obj)
        {
            var varsViewModel = new VarsViewModel(adapter.CompiledVars);
            var varsWindow = new VarsWindow(varsViewModel);
            varsWindow.ShowDialog();
        }


        /// <summary>
        /// Handles the PropertyChanged event of the BaseViewModel control and adds a subscrition to changes on <see cref="MainWindowViewModel.ShowVarsWindow"/>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void BaseViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.BaseViewModel_PropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case nameof(MainWindowViewModel.ShowVarsWindow):
                    OnPropertyChanged(nameof(ShowVarsWindow));
                    break;
                case nameof(MainWindowViewModel.HaveVSCodeInstalled):
                    OnPropertyChanged(nameof(HaveVSCodeInstalled));
                    break;
                case nameof(MainWindowViewModel.HavePSInstalled):
                    OnPropertyChanged(nameof(HavePSInstalled));
                    break;
                default:
                    break;
            }

        }

        public bool CanEditTemplate()
        {
            if (adapter.SelectedTemplate.IsEmpty())
            {
                return false;
            }

            try
            {
                string basePath = adapter.SettingsConfiguration["TemplatesBasePath"].AddPathSeparator();
                string template = adapter.TemplatesConfiguration[adapter.SelectedTemplate].Path;
                string TemplatePath = Path.Combine(basePath, template);
                return Directory.Exists(TemplatePath);
            }
            catch (Exception ex)
            {
                messageService.Error($"GeneratorAdapter.CanEditTemplate() : {ex.Message}", Messages.ErrorGettingTemplatePath);
                return false;
            }
        }

        public bool CanOpenSettings()
        {
            var appBasePath = AppDomain.CurrentDomain.BaseDirectory.TrimPathSeparator();
            var config = $"{appBasePath}\\appsettings.json";
            return File.Exists(config);
        }

        public bool CheckCanOpenVS(string slnPath, out string slnFile)
        {
            slnFile = null;
            if (slnPath == null) return false;

            slnFile = adapter.SlnFile.ToLower();
            if (File.Exists(slnFile))
            {
                return true;
            }
            slnFile = null;
            return false;
        }

        public void EditTemlate()
        {
            string basePath = adapter.SettingsConfiguration["TemplatesBasePath"].AddPathSeparator();
            string template = adapter.TemplatesConfiguration[adapter.SelectedTemplate].Path;
            string TemplatePath = Path.Combine(basePath, template);
            OpenWithEditor(TemplatePath);
        }

        public bool IsVSClosed(string slnPath)
        {
            return CheckCanOpenVS(slnPath, out _);
        }

        public void OpenSettings()
        {
            var appBasePath = AppDomain.CurrentDomain.BaseDirectory.TrimPathSeparator();
            var config = $"{appBasePath}\\appsettings.json";
            OpenWithEditor(config);
        }

        private void OpenWithEditor(string file)
        {
            string defaultEditor = mainWindowViewModel.VSCodePath;

            string editor1 = adapter.SettingsConfiguration["Editor"];
            string editor2 = adapter.SettingsConfiguration["Editor2"];

            editor1 = editor1 == "VSCODE" ? defaultEditor : editor1;
            editor2 = editor2 == "VSCODE" ? defaultEditor : editor2;

            var editor = !File.Exists(editor1) ? (!File.Exists(editor2) ? file : editor2) : editor1;
            var args = editor.Equals(file) ? string.Empty : file;

            ProcessManager.Run(editor, args);
        }



    }
}
