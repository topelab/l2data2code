using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace L2Data2Code.SharedLib.Helpers
{
    public enum LogType
    {
        Trace,
        Info,
        Warn,
        Error,
        Fatal
    }

    public static class LogService
    {
        private static readonly Dictionary<LogType, Action<string>> logAction = new();

        private static ILogger logger = null;
        public static ILogger Logger
        {
            get => logger;
            private set
            {
                if (value != null)
                {
                    logger = value;
                    logAction.Clear();
                    logAction.Add(LogType.Trace, logger.Trace);
                    logAction.Add(LogType.Info, logger.Info);
                    logAction.Add(LogType.Warn, logger.Warn);
                    logAction.Add(LogType.Error, logger.Error);
                    logAction.Add(LogType.Fatal, logger.Fatal);
                }
            }
        }

        public static bool DefaultRegisterCaller { get; set; } = false;

        public static string LastError { get; private set; }

        public static void Log(this string message, LogType logType = LogType.Info, bool? registerCaller = null)
        {
            LastError = message;
            string output;

            if (registerCaller ?? DefaultRegisterCaller || (logType != LogType.Info && logType != LogType.Warn))
            {
                var Terminal = $"{Environment.GetEnvironmentVariable("CLIENTNAME")} {Environment.GetEnvironmentVariable("COMPUTERNAME")}".Trim();
                var Usuario = Environment.GetEnvironmentVariable("USERNAME");
                output = $"{Usuario} {Terminal} [{GetTraceInfo()}]: {message}";
            }
            else
            {
                output = $"{message}";
            }

            GetAction(logType)?.Invoke(output);
        }

        public static void Error(string message) => message.Log(LogType.Error);

        public static void ToScreen(this string text)
        {
            text.ToScreen(null, LogType.Info);
        }

        public static void ToScreen(this string text, ConsoleColor color)
        {
            text.ToScreen(color, LogType.Info);
        }

        public static void ToScreen(this string text, LogType tipo)
        {
            text.ToScreen(null, tipo);
        }

        public static void ToScreen(this string text, ConsoleColor? color, LogType? tipo)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color ?? currentColor;
            Console.WriteLine(text);
            Console.ResetColor();

            if (tipo != null)
            {
                Log(text, tipo.Value);
            }
        }

        private static string GetTraceInfo(int maxFrames = 4)
        {

            var st = new StackTrace(true);
            int frames = st.FrameCount > maxFrames + 2 ? maxFrames + 2 : st.FrameCount;
            var sb = new StringBuilder();

            for (int i = frames - 1; i > 1; i--)
            {
                if (i < frames - 1)
                {
                    sb.Append(" > ");
                }
                var frame = st.GetFrame(i);
                var method = frame.GetMethod();
                sb.Append($"{method.DeclaringType.Name}.{method.Name}({frame.GetFileLineNumber()})");
            }

            return sb.ToString();
        }

        private static Action<string> GetAction(LogType logType)
        {
            if (Logger == null)
            {
                Logger = LogManager.GetCurrentClassLogger();
            }

            if (logAction.ContainsKey(logType))
            {
                return logAction[logType];
            }
            else
            {
                return null;
            }
        }
    }
}
