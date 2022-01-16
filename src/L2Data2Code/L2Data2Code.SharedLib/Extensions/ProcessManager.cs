using L2Data2Code.SharedLib.Helpers;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Topelab.Core.Resolver.Interfaces;

namespace L2Data2Code.SharedLib.Extensions
{
    public class ProcessManager : IProcessManager
    {
        private readonly ManagementObjectSearcher ProcessSearcher;
        private readonly Dictionary<string, Process> runningProcess = new();
        private IEnumerable<ProcessSearched> _allRunningEditors;
        private ILogger logger;
        private readonly IResolver resolver;

        [SupportedOSPlatform("windows")]
        public ProcessManager(ILogger logger, IResolver resolver)
        {
            this.logger = logger;
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            ProcessSearcher = new("SELECT ProcessId, CommandLine FROM Win32_Process WHERE CommandLine IS NOT NULL AND (Name = 'devenv.exe' OR Name = 'Code.exe')");
        }

        [SupportedOSPlatform("windows")]
        public async Task CheckEditorsOpenedAsync(string editor, string file = null, Action ifFileOpened = null, Action onExit = null)
        {
            if (editor.IsEmpty())
            {
                return;
            }

            var editorOpened = await GetAllRunningEditors();

            foreach (var item in editorOpened)
            {
                try
                {
                    var args = item.Args.Trim('\"');
                    var currentFileOpened = file != null && file.Equals(args, StringComparison.CurrentCultureIgnoreCase);
                    Process proc = Process.GetProcessById((int)item.Id);
                    Register(proc, GetKey(item.Program, args), currentFileOpened ? ifFileOpened : null, onExit);
                }
                catch (ArgumentException ex)
                {
                    logger.Error($"It seems {item.Id} is not running: {ex.Message}");
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"When registering editor {item.Args}");
                    throw;
                }
            }
        }

        [SupportedOSPlatform("windows")]
        public async Task<Process> CheckSolutionOpened(string slnFile = null, Action ifSolutionOpened = null, Action onExit = null)
        {
            var vsOpened = await GetAllRunningEditors();
            Process currentProcess = null;

            foreach (var item in vsOpened)
            {
                try
                {
                    var sln = item.Args.Trim('\"').ToLower();
                    var currentSlnOpened = slnFile != null && slnFile.Equals(sln);
                    Process proc = Process.GetProcessById((int)item.Id);
                    if (currentSlnOpened)
                    {
                        currentProcess = proc;
                    }
                    Register(proc, sln, currentSlnOpened ? ifSolutionOpened : null, currentSlnOpened ? onExit : null);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"When registering solution {item.Args}");
                    throw;
                }
            }
            return currentProcess;
        }

        public string FindPS()
        {
            var pwsh = WhereIsFile("PowerShell", "pwsh.exe");
            return pwsh ?? WhereIsFile("PowerShell", "powershell.exe");
        }

        public string FindVSCode()
        {
            return WhereIsFile("VS Code", $"..{Path.DirectorySeparatorChar}Code.exe");
        }

        public bool IsRunning(string program) => program != null && runningProcess.ContainsKey(program.ToLower());

        public void Run(string program) => Run(program, null, null, null);

        public void Run(string program, string arguments) => Run(program, arguments, null, null);

        public void Run(string program, string arguments, Action onNewProcess) => Run(program, arguments, onNewProcess, null);

        public void Run(string program, string arguments, Action onNewProcess, Action onExit)
        {
            Process proc;

            if (program.IsEmpty())
            {
                return;
            }

            logger ??= resolver.Get<ILogger>();

            var key = GetKey(program, arguments);
            logger.Info($"Running {key}");

            if (runningProcess.ContainsKey(key))
            {
                try
                {
                    proc = runningProcess[key];
                    if (!proc.HasExited)
                    {
                        SetForegroundWindow(proc.MainWindowHandle);
                        return;
                    }
                    RemoveFromRunning(proc);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"Removing or set foreground {key}");
                    throw;
                }
            }

            try
            {
                proc = Process.Start(new ProcessStartInfo(program, arguments) { UseShellExecute = true });
                Register(proc, key, onNewProcess, onExit);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Starting or registering {key}");
                throw;
            }
        }

        [SupportedOSPlatform("windows")]
        public async Task UpdateRunningEditors()
        {
            await Task.Run(() =>
            {
                using var objects = ProcessSearcher.Get();
                _allRunningEditors = objects.Cast<ManagementBaseObject>()
                    .Select(o => new ProcessSearched { Id = (uint)o["ProcessId"], Program = GetProgram(o["CommandLine"].ToString()), Args = GetArgs(o["CommandLine"].ToString()) })
                    .ToList();
            });
        }

        private static string GetKey(string program, string arguments) =>
            program.ToLower() + arguments.IfNotEmpty($" {arguments}");

        private static string GetProgram(string commandLine)
        {
            var result = commandLine.Trim();
            if (result.StartsWith("\""))
            {
                return result.Split('\"')[1].Trim();
            }
            else
            {
                return result.Split(' ')[0].Trim();
            }
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private static string WhereIsFile(string description, string file)
        {
            var paths = Environment.GetEnvironmentVariable("PATH")
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Where(s => s.Contains(description));

            var path = paths.Where(path => File.Exists($"{path}{Path.DirectorySeparatorChar}{file}")).FirstOrDefault();
            return path == null ? null : Path.GetFullPath(Path.Combine(path, file));
        }

        private string Arguments(string result, string search)
        {
            var firstPos = result.IndexOf(search);
            if (firstPos > -1)
            {
                return result[(firstPos + search.Length)..].Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        [SupportedOSPlatform("windows")]
        private async Task<IEnumerable<ProcessSearched>> GetAllRunningEditors(bool force = false)
        {
            if (force || _allRunningEditors == null)
            {
                await UpdateRunningEditors();
            }

            return _allRunningEditors;
        }

        private string GetArgs(string commandLine)
        {
            var result = commandLine.Trim();
            var startWithQuotes = result.StartsWith("\"");
            return Arguments(result, startWithQuotes ? "\" " : " ");
        }

        private void Register(Process proc, string program, Action onNewProcess, Action onExit)
        {
            if (proc != null && !proc.HasExited && proc.MainWindowHandle != IntPtr.Zero)
            {
                var oldProc = runningProcess.ContainsKey(program) ? runningProcess[program] : null;
                if (oldProc != proc)
                {
                    proc.EnableRaisingEvents = true;
                    proc.Exited += (s, e) =>
                    {
                        RemoveFromRunning((Process)s, onExit);
                    };
                    onNewProcess?.Invoke();
                }
                runningProcess[program] = proc;
            }
        }
        private void RemoveFromRunning(Process proc, Action onExit = null)
        {
            var key = runningProcess.Where(d => d.Value == proc).Select(d => d.Key).FirstOrDefault();

            if (key != null)
            {
                lock (runningProcess)
                {
                    runningProcess.Remove(key);
                }
                onExit?.Invoke();
            }
        }
    }
}