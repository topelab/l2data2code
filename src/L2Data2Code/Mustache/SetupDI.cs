using L2Data2Code.BaseMustache.Extensions;
using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.BaseMustache.Services;
using L2Data2Code.SharedLib.Helpers;
using NLog;
using Unity;

namespace Mustache
{
    public class SetupDI
    {

        private static IUnityContainer container;
        public static IUnityContainer Container => container ?? Register();

        private static IUnityContainer Register()
        {
            UnityContainer container = new();
            SetupDI.container = Register(container);
            return container;
        }

        private static IUnityContainer Register(IUnityContainer container)
        {
            container.RegisterSingleton<IJsonSetting, JsonSetting>();
            container.RegisterSingleton<IMustacheHelpers, MustacheHelpers>();
            container.RegisterSingleton<IMustacheRenderizer, MustacheRenderizer>();

            container.RegisterType<IMustacheAction, MustacheAction>();
            container.RegisterType<IFileExecutor, FileExecutor>();

            container.RegisterInstance<ILogger>(LogManager.GetCurrentClassLogger());
            return container;
        }
    }
}
