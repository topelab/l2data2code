using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Commands;
using L2Data2Code.SharedContext.Main;
using L2Data2Code.SharedContext.Main.CommandBar;
using L2Data2Code.SharedContext.Main.Interfaces;
using L2Data2Code.SharedContext.Main.MessagePanel;
using L2Data2Code.SharedContext.Main.TablePanel;
using Topelab.Core.Resolver.Entities;

namespace L2Data2Code.SharedContext
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
                .AddSingleton<ICommandBarFactory, CommandBarFactory>()
                .AddSingleton<ITablePanelFactory, TablePanelFactory>()
                .AddSingleton<IMessagePanelFactory, MessagePanelFactory>()
                .AddSingleton<IMessagePanelService, MessagePanelService>()

                .AddTransient<MainWindowVM, MainWindowVM>()
                .AddTransient<IMainWindowEventManager, MainWindowEventManager>()
                .AddTransient<IMainWindowVMChangeListener, MainWindowVMChangeListener>()
                .AddTransient<IMainWindowVMInitializer, MainWindowVMInitializer>()
                .AddTransient<ICommandBarChangeListener, CommandBarChangeListener>()
                .AddTransient<ITablePanelChangeListener, TablePanelChangeListener>()
                .AddTransient<IMessagePanelChangeListener, MessagePanelChangeListener>()
                .AddTransient<IGenerateCommandFactory, GenerateCommandFactory>()
                .AddTransient<IEditTemplateCommandFactory, EditTemplateCommandFactory>()
                .AddTransient<IOpenFolderCommandFactory, OpenFolderCommandFactory>()
                .AddTransient<IOpenPSCommandFactory, OpenPSCommandFactory>()
                .AddTransient<IOpenSettingsCommandFactory, OpenSettingsCommandFactory>()
                .AddTransient<IOpenVarsWindowCommandFactory, OpenVarsWindowCommandFactory>()
                .AddTransient<IOpenVSCodeCommandFactory, OpenVSCodeCommandFactory>()
                .AddTransient<IOpenVSCommandFactory, OpenVSCommandFactory>()
                .AddTransient<ILoadTablesCommandFactory, LoadTablesCommandFactory>()
                .AddTransient<ISetDataItemsCommandFactory, SetDataItemsCommand>()
                .AddTransient<ISetDataItemCommandFactory, SetDataItemCommandFactory>()
                ;
        }
    }
}
