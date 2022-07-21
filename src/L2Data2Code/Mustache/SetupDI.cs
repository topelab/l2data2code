using L2Data2Code.BaseMustache;
using L2Data2Code.CLIBase.Interfaces;
using L2Data2Code.CLIBase.Options;
using L2Data2Code.CLIBase.Services;
using L2Data2Code.SharedLib.Helpers;
using L2Data2Code.SharedLib.Interfaces;
using L2Data2Code.SharedLib.Services;
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
                .AddSingleton<ICLIOptionsInitializer, CLIOptionsInitializer>()
                .AddSingleton<IConditionalPathRenderizer, ConditionalPathRenderizer>()
                .AddSingleton<IMultiPathRenderizer, MultiPathRenderizer>()
                .AddSingleton<IFileService, FileService>()

                .Add<ICLIAction, CLIAction>()
                .Add<IFileExecutor, FileExecutor>()
                .AddInstance<ILogger>(LogManager.GetCurrentClassLogger());
        }
    }
}
