using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Helpers;
using NLog;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Unity;

namespace L2Data2CodeWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ILogger Logger { get; private set; }

        public static bool RestartApp { get; set; }


        public App()
        {
            var container = ContainerManager.SetupContainer(SetupDI.Container);
            Logger = container.Resolve<ILogger>();

            var settings = container.Resolve<IAppSettingsConfiguration>();

            var uiCulture = settings["UICulture"];
            if (uiCulture != null && !uiCulture.Equals("auto", StringComparison.CurrentCultureIgnoreCase))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(uiCulture);
            }
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error("Current_DispatcherUnhandledException");
            Logger.Error(e.Exception);
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
