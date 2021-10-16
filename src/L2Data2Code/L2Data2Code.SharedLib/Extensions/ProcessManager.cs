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
using Unity;

namespace L2Data2Code.SharedLib.Extensions
{
    public class ProcessSearched
    {
        public uint Id { get; set; }
        public string Program { get; set; }
        public string Args { get; set; }
    }

    public static class ProcessManager
    {
        private static readonly Dictionary<string, Process> runningProcess = new();
        private static ILogger logger;

        public static void Run(this string program) => Run(program, null, null, null);

        public static void Run(this string program, string arguments) => Run(program, arguments, null, null);

        public static void Run(this string program, string arguments, Action onNewProcess) => Run(program, arguments, onNewProcess, null);

        public static void Run(this string program, string arguments, Action onNewProcess, Action onExit)
        {
            Process proc;

            if (program.IsEmpty())
            {
                return;
            }

            logger ??= ContainerManager.Container.Resolve<ILogger>();

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

        private static string GetKey(string program, string arguments) =>
            program.ToLower() + arguments.IfNotEmpty($" {arguments}");

        private static void Register(this Process proc, string program, Action onNewProcess, Action onExit)
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

        public static void Activate(IntPtr hWnd) => SetForegroundWindow(hWnd);

        public static bool IsRunning(string program) => program != null && runningProcess.ContainsKey(program.ToLower());

        [SupportedOSPlatform("windows")]
        public async static Task<Process> CheckSolutionOpened(string slnFile = null, Action ifSolutionOpened = null, Action onExit = null)
        {
            var vsOpened = await GetAllRunningEditors();
            Process currentProcess = null;

            foreach (var item in vsOpened)
            {
                try
                {
                    var sln = item.Args.Trim('\"').ToLower();
                    bool currentSlnOpened = slnFile != null && slnFile.Equals(sln);
                    var proc = Process.GetProcessById((int)item.Id);
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

        [SupportedOSPlatform("windows")]
        public static async Task CheckEditorsOpenedAsync(string editor, string file = null, Action ifFileOpened = null, Action onExit = null)
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
                    bool currentFileOpened = file != null && file.Equals(args, StringComparison.CurrentCultureIgnoreCase);
                    var proc = Process.GetProcessById((int)item.Id);
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
        private static readonly ManagementObjectSearcher ProcessSearcher =
            new("SELECT ProcessId, CommandLine FROM Win32_Process WHERE CommandLine IS NOT NULL AND (Name = 'devenv.exe' OR Name = 'Code.exe')");

        private static IEnumerable<ProcessSearched> _allRunningEditors;

        [SupportedOSPlatform("windows")]
        public async static Task<IEnumerable<ProcessSearched>> GetAllRunningEditors(bool force = false)
        {
            if (force || _allRunningEditors == null)
            {
                await UpdateRunningEditors();
            }

            return _allRunningEditors;
        }

        [SupportedOSPlatform("windows")]
        public async static Task UpdateRunningEditors()
        {
            await Task.Run(() =>
            {
                using ManagementObjectCollection objects = ProcessSearcher.Get();
                _allRunningEditors = objects.Cast<ManagementBaseObject>()
                    .Select(o => new ProcessSearched { Id = (uint)o["ProcessId"], Program = o["CommandLine"].ToString().GetProgram(), Args = o["CommandLine"].ToString().GetArgs() })
                    .ToList();
            });
        }

        [SupportedOSPlatform("windows")]
        public async static Task<IEnumerable<ProcessSearched>> GetCommandLine(this string processName)
        {
            return await Task.Run<IEnumerable<ProcessSearched>>(() =>
            {
                using ManagementObjectSearcher searcher = new("SELECT ProcessId, CommandLine FROM Win32_Process WHERE Name like '" + processName + "%' AND CommandLine IS NOT NULL");
                using ManagementObjectCollection objects = searcher.Get();
                return objects.Cast<ManagementBaseObject>()
                    .Select(o => new ProcessSearched { Id = (uint)o["ProcessId"], Program = o["CommandLine"].ToString().GetProgram(), Args = o["CommandLine"].ToString().GetArgs() })
                    .ToList();
            });
        }

        public static string GetProgram(this string commandLine)
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

        public static string GetArgs(this string commandLine)
        {
            var result = commandLine.Trim();
            bool startWithQuotes = result.StartsWith("\"");
            return result.Arguments(startWithQuotes ? "\" " : " ");
        }

        public static string WhereIsFile(string description, string file)
        {
            var paths = Environment.GetEnvironmentVariable("PATH")
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Where(s => s.Contains(description));

            var path = paths.Where(path => File.Exists($"{path}{Path.DirectorySeparatorChar}{file}")).FirstOrDefault();
            return path == null ? null : Path.GetFullPath(Path.Combine(path, file));
        }

        public static string FindPS()
        {
            var pwsh = WhereIsFile("PowerShell", "pwsh.exe");
            return pwsh ?? WhereIsFile("PowerShell", "powershell.exe");
        }

        public static string FindVSCode()
        {
            return WhereIsFile("VS Code", $"..{Path.DirectorySeparatorChar}Code.exe");
        }

        private static string Arguments(this string result, string search)
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

        private static void RemoveFromRunning(Process proc, Action onExit = null)
        {
            string key = runningProcess.Where(d => d.Value == proc).Select(d => d.Key).FirstOrDefault();

            if (key != null)
            {
                lock (runningProcess)
                {
                    runningProcess.Remove(key);
                }
                onExit?.Invoke();
            }
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static Task CheckEditorsOpenedAsync(object p)
        {
            throw new NotImplementedException();
        }
    }
}