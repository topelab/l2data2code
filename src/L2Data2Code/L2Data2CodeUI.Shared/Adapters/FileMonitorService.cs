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
            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Changed -= FileSystemWatcher_Changed;
            this.action = action;

            fileSystemWatcher.Path = basePath;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileSystemWatcher.Filters.Clear();
            if (filters != null)
            {
                foreach (var item in filters)
                {
                    fileSystemWatcher.Filters.Add(item);
                }
            }
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            FileSystemWatcher watcher = sender as FileSystemWatcher;
            watcher.EnableRaisingEvents = false;
            action.Invoke(e.Name);
            watcher.EnableRaisingEvents = true;
        }
    }
}
