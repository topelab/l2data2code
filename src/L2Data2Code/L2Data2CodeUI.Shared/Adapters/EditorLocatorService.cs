using L2Data2Code.SharedLib.Extensions;
using System;
using System.IO;

namespace L2Data2CodeUI.Shared.Adapters
{
    public class EditorLocatorService : IEditorLocatorService
    {
        private readonly IGeneratorAdapter adapter;
        private readonly IProcessManager processManager;

        public EditorLocatorService(IGeneratorAdapter adapter, IProcessManager processManager)
        {
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            this.processManager = processManager ?? throw new ArgumentNullException(nameof(processManager));
        }

        public void OpenWithEditor(string file)
        {
            string defaultEditor = processManager.FindVSCode();
            var editor1 = adapter.SettingsConfiguration["Editor"];
            var editor2 = adapter.SettingsConfiguration["Editor2"];

            editor1 = editor1 == "VSCODE" ? defaultEditor : editor1;
            editor2 = editor2 == "VSCODE" ? defaultEditor : editor2;

            var editor = !File.Exists(editor1) ? (!File.Exists(editor2) ? file : editor2) : editor1;
            var args = editor.Equals(file) ? string.Empty : file;

            processManager.Run(editor, args);
        }
    }
}
