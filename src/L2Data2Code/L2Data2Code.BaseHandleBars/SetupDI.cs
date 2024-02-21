using L2Data2Code.SharedLib.Interfaces;
using Topelab.Core.Resolver.Entities;

namespace L2Data2Code.BaseHandleBars
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
                .AddSingleton<IMustacheRenderizer, HandleBarsRenderizer>()
                ;
        }
    }
}
