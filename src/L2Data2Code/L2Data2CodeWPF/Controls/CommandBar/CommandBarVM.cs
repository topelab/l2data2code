using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Localize;
using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.Main;
using L2Data2CodeWPF.Vars;
using System;
using System.ComponentModel;
using System.IO;

namespace L2Data2CodeWPF.Controls.CommandBar
{
    public class CommandBarVM : BaseControlVM
    {
        private readonly IGeneratorAdapter adapter;
        private readonly MainWindowVM mainWindowVM;
        private readonly IMessageService messageService;
        private readonly IAppService appService;


        public CommandBarVM(IBaseVM baseVM) : base(baseVM)
        {
            mainWindowVM = (MainWindowVM)baseVM;
            adapter = mainWindowVM.Adapter;
            messageService = mainWindowVM.MessageService;
            appService = mainWindowVM.AppService;
        }

        public bool ChangeButtons { get; internal set; }

        public bool ShowVarsWindow
        {
            get { return mainWindowVM.ShowVarsWindow; }
            set { mainWindowVM.ShowVarsWindow = value; }
        }

        public bool HaveVSCodeInstalled => mainWindowVM.HaveVSCodeInstalled;
        public bool HavePSInstalled => mainWindowVM.HavePSInstalled;

        /// <summary>
        /// Gets the open folder command.
        /// </summary>
        /// <value>
        /// The open folder command.
        /// </value>
        public DelegateCommand OpenFolderCommand => new(OnOpenFolderCommand, CanOpenFolderCommand);

        private bool CanOpenFolderCommand(object arg)
        {
            return Directory.Exists(Path.GetDirectoryName(mainWindowVM.SlnFile));
        }

        private void OnOpenFolderCommand(object obj)
        {
            Working = true;
            ProcessManager.Run(Path.GetDirectoryName(mainWindowVM.SlnFile));
            Working = false;
        }

        /// <summary>
        /// Gets the edit template command.
        /// </summary>
        /// <value>
        /// The edit template command.
        /// </value>
        public DelegateCommand EditTemplateCommand => new(OnEditTemplateCommand, CanEditTemplateCommand);

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
        public DelegateCommand OpenSettingsCommand => new(OnOpenSettingsCommand, CanOpenSettingsCommand);

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
        public DelegateCommand OpenVSCommand => new(OnOpenVSCommand, CanOpenVSCommand);

        private bool CanOpenVSCommand(object arg)
        {
            return CheckCanOpenVS(mainWindowVM.OutputPath, out _);
        }

        private void OnOpenVSCommand(object obj)
        {
            if (CheckCanOpenVS(mainWindowVM.OutputPath, out var slnFile))
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
        public DelegateCommand OpenVSCodeCommand => new(OnOpenVSCodeCommand, CanOpenVSCodeCommand);

        private bool CanOpenVSCodeCommand(object arg)
        {
            return CheckCanOpenVS(mainWindowVM.OutputPath, out _) && mainWindowVM.HaveVSCodeInstalled;
        }

        private void OnOpenVSCodeCommand(object obj)
        {
            if (CheckCanOpenVS(mainWindowVM.OutputPath, out var slnFile))
            {
                appService.Open(slnFile, mainWindowVM.VSCodePath, AppService.DIRECTORY_PATTERN);
            }
        }

        /// <summary>
        /// Gets the open ps command.
        /// </summary>
        /// <value>
        /// The open ps command.
        /// </value>
        public DelegateCommand OpenPSCommand => new(OnOpenPSCommand, CanOpenPSCommand);

        private bool CanOpenPSCommand(object arg)
        {
            return CheckCanOpenVS(mainWindowVM.OutputPath, out _);
        }

        private void OnOpenPSCommand(object obj)
        {
            if (CheckCanOpenVS(mainWindowVM.OutputPath, out var slnFile))
            {
                appService.Open(slnFile, mainWindowVM.PSPath, $"-noexit -command \"cd {AppService.DIRECTORY_PATTERN}\"");
            }
        }

        /// <summary>
        /// Gets the open vars window.
        /// </summary>
        /// <value>
        /// The open vars window.
        /// </value>
        public DelegateCommand OpenVarsWindow => new(OnOpenVarsWindow, CanOpenVarsWindow);

        private bool CanOpenVarsWindow(object arg)
        {
            return true;
        }

        private void OnOpenVarsWindow(object obj)
        {
            VarsVM varsVM = new(adapter.CompiledVars);
            VarsWindow varsWindow = new(varsVM);
            varsWindow.ShowDialog();
        }


        /// <summary>
        /// Handles the PropertyChanged event of the BaseVM control and adds a subscrition to changes on <see cref="MainWindowVM.ShowVarsWindow"/>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void BaseVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.BaseVM_PropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case nameof(MainWindowVM.ShowVarsWindow):
                    OnPropertyChanged(nameof(ShowVarsWindow));
                    break;
                case nameof(MainWindowVM.HaveVSCodeInstalled):
                    OnPropertyChanged(nameof(HaveVSCodeInstalled));
                    break;
                case nameof(MainWindowVM.HavePSInstalled):
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
                var basePath = adapter.SettingsConfiguration["TemplatesBasePath"].AddPathSeparator();
                var template = adapter.TemplatesConfiguration[adapter.SelectedTemplate].Path;
                var TemplatePath = Path.Combine(basePath, template);
                return Directory.Exists(TemplatePath);
            }
            catch (Exception ex)
            {
                messageService.Error($"GeneratorAdapter.CanEditTemplate() : {ex.Message}", Messages.ErrorGettingTemplatePath);
                return false;
            }
        }

        public static bool CanOpenSettings()
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
            var basePath = adapter.SettingsConfiguration["TemplatesBasePath"].AddPathSeparator();
            var template = adapter.TemplatesConfiguration[adapter.SelectedTemplate].Path;
            var TemplatePath = Path.Combine(basePath, template);
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
            var defaultEditor = mainWindowVM.VSCodePath;

            var editor1 = adapter.SettingsConfiguration["Editor"];
            var editor2 = adapter.SettingsConfiguration["Editor2"];

            editor1 = editor1 == "VSCODE" ? defaultEditor : editor1;
            editor2 = editor2 == "VSCODE" ? defaultEditor : editor2;

            var editor = !File.Exists(editor1) ? (!File.Exists(editor2) ? file : editor2) : editor1;
            var args = editor.Equals(file) ? string.Empty : file;

            ProcessManager.Run(editor, args);
        }



    }
}
