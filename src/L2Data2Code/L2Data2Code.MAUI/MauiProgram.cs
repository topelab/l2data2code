using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Topelab.Core.Resolver.Entities;
using Topelab.Core.Resolver.Microsoft;

namespace L2Data2Code.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("materialdesignicons-webfont.ttf", "MaterialDesignIcons");
                });

            builder.Services.AddResolver(new ResolveInfoCollection());

#if DEBUG
		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}