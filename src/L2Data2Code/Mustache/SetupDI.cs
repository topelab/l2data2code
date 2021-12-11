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
        public static IUnityContainer Register(IUnityContainer container = null)
        {
            container ??= new UnityContainer();
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
