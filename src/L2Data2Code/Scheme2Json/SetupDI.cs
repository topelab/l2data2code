using Topelab.Core.Resolver.Entities;

namespace Scheme2Json
{
    /// <summary>
    /// Setup dependency injection
    /// </summary>
    public class SetupDI
    {
        /// <summary>
        /// Register interfaces
        /// </summary>
        /// <returns>Resolve info collection</returns>
        public static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddCollection(L2Data2Code.SharedLib.SetupDI.Register())
                .AddCollection(L2Data2Code.BaseGenerator.SetupDI.Register())
                .AddCollection(L2Data2Code.SchemaReader.SetupDI.Register())
                ;
        }
    }
}
