using L2Data2Code.MAUI.Main;
using L2Data2Code.SharedLib.Configuration;
using NLog;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Topelab.Core.Resolver.Interfaces;
using Topelab.Core.Resolver.Microsoft;

namespace L2Data2Code.MAUI
{
    public partial class App : Application
    {
        public static IResolver Resolver { get; private set; }
        public static ILogger Logger { get; private set; }
        public static bool RestartApp { get; internal set; }

        public App()
        {
            Directory.SetCurrentDirectory($"{AppDomain.CurrentDomain.BaseDirectory}\\..");
            Resolver = ResolverFactory.Create(SetupDI.Register());
            Logger = Resolver.Get<ILogger>();
            var settings = Resolver.Get<IAppSettingsConfiguration>();

            var uiCulture = settings["UICulture"];
            if (uiCulture != null && !uiCulture.Equals("auto", StringComparison.CurrentCultureIgnoreCase))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(uiCulture);
            }

            InitializeComponent();
            var pageFactory = Resolver.Get<IMainPageFactory>();
            MainPage = pageFactory.Create();
        }

        public override void CloseWindow(Window window)
        {
            Logger.Info("Application ending");
            if (RestartApp)
            {
                Logger.Info($"Restarting application at {Assembly.GetExecutingAssembly().Location}");
                Process.Start(Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "exe"));
            }
            base.CloseWindow(window);
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            const int newWidth = 1024;
            const int newHeight = 860;

            window.Width = newWidth;
            window.Height = newHeight;

            return window;
        }
    }
}