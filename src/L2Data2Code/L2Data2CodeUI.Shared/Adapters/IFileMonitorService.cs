using System;

namespace L2Data2CodeUI.Shared.Adapters
{
    public interface IFileMonitorService
    {
        void StartMonitoring(Action<string> action, string basePath, params string[] filters);
        void StopMonitoring();
    }
}