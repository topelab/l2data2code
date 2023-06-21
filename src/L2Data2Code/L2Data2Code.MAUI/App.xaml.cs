using NLog;
using Topelab.Core.Resolver.Interfaces;

namespace L2Data2Code.MAUI
{
    public partial class App : Application
    {
        public static IResolver Resolver { get; private set; }
        public static ILogger Logger { get; private set; }
        public static bool RestartApp { get; internal set; }

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }
    }
}