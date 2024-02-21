using L2Data2CodeUI.Shared.Adapters;
using Topelab.Core.Resolver.Entities;

namespace L2Data2CodeUI.Shared
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
                .AddSingleton<IEditorLocatorService, EditorLocatorService>()
                .AddSingleton<IMessageService, MessageService>()
                .AddSingleton<IAppService, AppService>()
                .AddSingleton<ICommandService, CommandService>()
                .AddSingleton<IGeneratorAdapter, GeneratorAdapter>()
                .AddSingleton<IGitService, GitService>()
                .AddTransient<IFileMonitorService, FileMonitorService>()
                ;
        }
    }
}
