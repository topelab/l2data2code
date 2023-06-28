using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using L2Data2Code.Avalonia.Main;
using L2Data2Code.SharedLib.Configuration;
using NLog;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using Topelab.Core.Resolver.Interfaces;
using Topelab.Core.Resolver.Microsoft;

namespace L2Data2Code
{
    public partial class App : Application
    {
        public static IResolver Resolver { get; private set; }
        public static ILogger Logger { get; private set; }
        public static bool RestartApp { get; internal set; }

        public override void Initialize()
        {
            Resolver = ResolverFactory.Create(SetupDI.Register());
            Logger = Resolver.Get<ILogger>();
            var settings = Resolver.Get<IAppSettingsConfiguration>();

            var uiCulture = settings["UICulture"];
            if (uiCulture != null && !uiCulture.Equals("auto", StringComparison.CurrentCultureIgnoreCase))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(uiCulture);
            }
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var windowFactory = Resolver.Get<IMainWindowFactory>();
                desktop.MainWindow = windowFactory.Create();
                desktop.MainWindow.Closed += OnMainWindowClosed;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            ((Window)sender).Closed -= OnMainWindowClosed;
            Logger.Info("Application ending");
            if (RestartApp)
            {
                Logger.Info($"Restarting application at {Assembly.GetExecutingAssembly().Location}");
                Process.Start(Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "exe"));
            }
        }
    }
}
