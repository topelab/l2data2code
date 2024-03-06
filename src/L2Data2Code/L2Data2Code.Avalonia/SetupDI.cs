using L2Data2Code.Avalonia.Base;
using L2Data2Code.Avalonia.Main;
using L2Data2Code.Avalonia.Main.Vars;
using L2Data2Code.SharedContext.Base;
using Topelab.Core.Resolver.Entities;

namespace L2Data2Code
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
                .AddCollection(SharedLib.SetupDI.Register())
                .AddCollection(BaseGenerator.SetupDI.Register())
                .AddCollection(SchemaReader.SetupDI.Register())
                .AddCollection(SharedContext.SetupDI.Register())
                .AddCollection(L2Data2CodeUI.Shared.SetupDI.Register())
                .AddCollection(BaseHandleBars.SetupDI.Register())

                .AddSingleton<IDispatcherWrapper, DispatcherWrapper>()
                .AddSingleton<IMessageBoxWrapper, MessageBoxWrapper>()
                .AddSingleton<IMainWindowFactory, MainWindowFactory>()
                .AddSingleton<IVarsFactory, VarsFactory>()
                .AddSingleton<IGlobalEventManager, GlobalEventManager>()
                ;
        }
    }
}
