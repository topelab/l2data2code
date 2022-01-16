using L2Data2Code.SharedLib.Configuration;
using NLog;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Topelab.Core.Resolver.Interfaces;
using Topelab.Core.Resolver.Microsoft;

namespace L2Data2CodeWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ILogger Logger { get; private set; }

        public static bool RestartApp { get; set; }

        public static IResolver Resolver { get; private set; }


        public App()
        {
            Resolver = ResolverFactory.Create(SetupDI.Register());
            Logger = Resolver.Get<ILogger>();
            var settings = Resolver.Get<IAppSettingsConfiguration>();

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
