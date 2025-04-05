using L2Data2Code.BaseGenerator.Configuration;
using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.BaseGenerator.Services;
using L2Data2Code.SharedLib.Configuration;
using Topelab.Core.Resolver.Entities;

namespace L2Data2Code.BaseGenerator
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
                .AddSingleton<IDataSorcesConfiguration, DataSourcesConfiguration>()
                .AddSingleton<IGlobalsConfiguration, GlobalsConfiguration>()
                .AddSingleton<IBasicConfiguration<ModuleConfiguration>, ModulesConfiguration>()
                .AddSingleton<ITemplatesConfiguration, TemplatesConfiguration>()
                .AddSingleton<ICodeGeneratorService, CodeGeneratorService>()
                .AddSingleton<ITemplateService, TemplateService>()
                .AddSingleton<IReplacementCollectionFactory, ReplacementCollectionFactory>()
                .AddSingleton<IEntityTablesFactory, EntityTablesFactory>()
                ;
        }

    }
}
