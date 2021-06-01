using L2Data2Code.SharedLib.Configuration;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace L2Data2CodeWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ILogger Logger { get; private set; } = LogManager.GetCurrentClassLogger();

        public static bool RestartApp { get; set; }


        public App()
        {
            var settings = new AppSettingsConfiguration();
            var uiCulture = settings["UICulture"];
            if (uiCulture != null && !uiCulture.Equals("auto", System.StringComparison.CurrentCultureIgnoreCase))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(uiCulture);
            }
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error("Current_DispatcherUnhandledException");
            Logger.Error(e.Exception);
            // e.Handled = true;
        }

    protected override void OnExit(ExitEventArgs e)
        {
            Logger.Info("Application ending");
            if (RestartApp)
            {
                Logger.Info($"Restarting application at {Assembly.GetExecutingAssembly().Location}");
                Process.Start(Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "exe"));
            }
            base.OnExit(e);
        }
    }
}
