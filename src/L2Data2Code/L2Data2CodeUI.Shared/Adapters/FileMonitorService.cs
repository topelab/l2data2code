using System;
using System.IO;

namespace L2Data2CodeUI.Shared.Adapters
{
    public class FileMonitorService : IFileMonitorService
    {
        private readonly FileSystemWatcher fileSystemWatcher;
        private Action<string> action;

        public FileMonitorService()
        {
            fileSystemWatcher = new FileSystemWatcher();
        }

        public void StartMonitoring(Action<string> action, string basePath, params string[] filters)
        {
            StopMonitoring();

            if (Path.Exists(basePath))
            {
                this.action = action;
                fileSystemWatcher.Path = basePath;
                fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName;
                fileSystemWatcher.Filters.Clear();
                if (filters != null)
                {
                    foreach (var item in filters)
                    {
                        fileSystemWatcher.Filters.Add(item);
                    }
                }
                fileSystemWatcher.Changed += OnFileSystemWatcher;
                fileSystemWatcher.Deleted += OnFileSystemWatcher;
                fileSystemWatcher.Created += OnFileSystemWatcher;

                fileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        public void StopMonitoring()
        {
            fileSystemWatcher.Deleted -= OnFileSystemWatcher;
            fileSystemWatcher.Changed -= OnFileSystemWatcher;
            fileSystemWatcher.Created -= OnFileSystemWatcher;
            fileSystemWatcher.EnableRaisingEvents = false;
        }

        private void OnFileSystemWatcher(object sender, FileSystemEventArgs e)
        {
            var watcher = sender as FileSystemWatcher;
            if (watcher.EnableRaisingEvents)
            {
                watcher.EnableRaisingEvents = false;
                action.Invoke(e.Name);
                watcher.EnableRaisingEvents = true;
            }
        }
    }
}
