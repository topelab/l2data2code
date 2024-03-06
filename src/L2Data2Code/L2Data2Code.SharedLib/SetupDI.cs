using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using L2Data2Code.SharedLib.Interfaces;
using L2Data2Code.SharedLib.Services;
using NLog;
using Prism.Events;
using Topelab.Core.Resolver.Entities;

namespace L2Data2Code.SharedLib
{
    public class SetupDI
    {
        private static bool IsLoaded;

        public static ResolveInfoCollection Register()
        {
            if (IsLoaded)
            {
                return [];
            }

            IsLoaded = true;

            return new ResolveInfoCollection()
                .AddSingleton<IJsonSetting, JsonSetting>()
                .AddSingleton<IAppSettingsConfiguration, AppSettingsConfiguration>()
                .AddSingleton<IConditionalPathRenderizer, ConditionalPathRenderizer>()
                .AddSingleton<IFileService, FileService>()
                .AddSingleton<IProcessManager, ProcessManager>()
                .AddSingleton<IEventAggregator, EventAggregator>()
                .AddInstance<ILogger>(LogManager.GetCurrentClassLogger())
                ;
        }
    }
}
