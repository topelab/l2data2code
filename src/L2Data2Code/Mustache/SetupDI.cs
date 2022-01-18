using L2Data2Code.BaseMustache.Extensions;
using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.BaseMustache.Services;
using L2Data2Code.SharedLib.Helpers;
using NLog;
using Topelab.Core.Resolver.Entities;
using Topelab.Core.Resolver.Enums;

namespace Mustache
{
    public class SetupDI
    {
        public static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .Add<IJsonSetting, JsonSetting>(ResolveLifeCycleEnum.Singleton)
                .Add<IMustacheHelpers, MustacheHelpers>(ResolveLifeCycleEnum.Singleton)
                .Add<IMustacheRenderizer, MustacheRenderizer>(ResolveLifeCycleEnum.Singleton)

                .Add<IMustacheAction, MustacheAction>()
                .Add<IFileExecutor, FileExecutor>()
                .Add<ILogger, Logger>(LogManager.GetCurrentClassLogger());
        }
    }
}