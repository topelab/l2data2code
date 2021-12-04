using System;
using System.IO;

namespace Mustache
{
    internal class FileExecutor : IFileExecutor
    {
        private string[] files;
        private string[] dirs;

        public void Initialize(string templatePath)
        {
            templatePath = templatePath ?? throw new ArgumentNullException(nameof(templatePath));
            files = Directory.GetFiles(templatePath, "*.*", SearchOption.AllDirectories);
            dirs = Directory.GetDirectories(templatePath, "*.*", SearchOption.AllDirectories);
        }

        public void Run(Action<string> actionForPaths, Action<string> actionForFiles)
        {
            foreach (var item in dirs)
            {
                actionForPaths.Invoke(item);
            }

            foreach (var item in files)
            {
                actionForFiles.Invoke(item);
            }
        }
    }
}