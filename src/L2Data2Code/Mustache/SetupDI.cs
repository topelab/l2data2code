using L2Data2Code.BaseMustache.Extensions;
using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.BaseMustache.Services;
using L2Data2Code.SharedLib.Helpers;
using NLog;
using Topelab.Core.Resolver.Entities;

namespace Mustache
{
    public class SetupDI
    {
        public static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddSingleton<IJsonSetting, JsonSetting>()
                .AddSingleton<IMustacheHelpers, MustacheHelpers>()
                .AddSingleton<IMustacheRenderizer, MustacheRenderizer>()
                .AddSingleton<IMustacheOptionsInitializer, MustacheOptionsInitializer>()

                .Add<IMustacheAction, MustacheAction>()
                .Add<IFileExecutor, FileExecutor>()
                .AddInstance<ILogger>(LogManager.GetCurrentClassLogger());
        }
    }
}
