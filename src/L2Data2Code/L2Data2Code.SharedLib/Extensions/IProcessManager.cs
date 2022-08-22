using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace L2Data2Code.SharedLib.Extensions
{
    public interface IProcessManager
    {
        Task CheckEditorsOpenedAsync(string editor, string file = null, Action ifFileOpened = null, Action onExit = null);
        Task<Process> CheckSolutionOpened(string slnFile = null, Action ifSolutionOpened = null, Action onExit = null);
        string FindPS();
        string FindVSCode();
        bool IsRunning(string program);
        void Run(string program);
        void Run(string program, string arguments);
        void Run(string program, string arguments, Action onNewProcess);
        void Run(string program, string arguments, Action onNewProcess, Action onExit);
        Task UpdateRunningEditors();
    }
}