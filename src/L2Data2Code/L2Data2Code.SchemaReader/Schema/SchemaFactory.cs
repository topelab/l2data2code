using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Fake;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Json;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.MySql;
using L2Data2Code.SchemaReader.Object;
using L2Data2Code.SchemaReader.SqlServer;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Helpers;
using System;
using System.Collections.Generic;
using Topelab.Core.Resolver.Interfaces;

namespace L2Data2Code.SchemaReader.Schema
{
    public class SchemaFactory : ISchemaFactory
    {
        internal class ProviderDefinition
        {
            public string Key { get; set; }
            public Type Type { get; set; }
            public Dictionary<string, string> Conversions { get; set; }
        }

        private readonly Dictionary<string, ProviderDefinition> providers = new()
        {
            { "System.Data.SqlClient", new ProviderDefinition { Key = "sqlserver", Type = typeof(SqlServerSchemaReader) } },
            { "MySql.Data.MySqlClient", new ProviderDefinition { Key = "mysql", Type = typeof(MySqlSchemaReader) } },
            { "System.Data.FakeClient", new ProviderDefinition { Key = "fake", Type = typeof(FakeSchemaReader) } },
            { "System.Data.JsonClient", new ProviderDefinition { Key = "json", Type = typeof(JsonSchemaReader) } },
            { "Microsoft.Data.Sqlite", new ProviderDefinition { Key = "sqlite", Type = typeof(SQLiteSchemaReader), Conversions = new Dictionary<string, string>() {{ "decimal", "NUMERIC" } } } },
            { "System.Data.ObjectClient", new ProviderDefinition { Key = "object", Type = typeof(ObjectSchemaReader) } },
            //{ "Oracle.ManagedDataAccess.Client", typeof(OracleSchemaReader) },

        };

        private IBasicConfiguration<SchemaConfiguration> schemasConfiguration;

        private readonly INameResolver nameResolver;
        private readonly IResolver resolver;

        public SchemaFactory(INameResolver nameResolver, IResolver resolver)
        {
            this.nameResolver = nameResolver ?? throw new ArgumentNullException(nameof(nameResolver));
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public string GetProviderDefinitionKey(string connectionStringKey)
        {
            Connection connection = new(schemasConfiguration, connectionStringKey);
            return providers.TryGetValue(connection.Provider, out var providerDefinition) ? providerDefinition.Key : null;
        }

        public ISchemaReader Create(ISchemaOptions schemaOptions)
        {
            Connection connection;
            Connection commentConnection;
            try
            {
                schemaOptions.SummaryWriter ??= new StringBuilderWriter();
                schemasConfiguration = schemaOptions.SchemasConfiguration;
                connection = new Connection(schemasConfiguration, schemaOptions.SchemaName);
                schemaOptions.ConnectionString = connection.ConnectionString;
                commentConnection = schemaOptions.DescriptionsSchemaName == null ? null : new Connection(schemasConfiguration, schemaOptions.DescriptionsSchemaName);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
                return null;
            }

            if (commentConnection != null && providers.ContainsKey(commentConnection.Provider))
            {
                schemaOptions.DescriptionsConnectionString = commentConnection.ConnectionString;
            }

            if (providers.TryGetValue(connection.Provider, out var providerDefinition))
            {
                return resolver.Get<ISchemaReader, INameResolver, ISchemaOptions>(providerDefinition.Type.Name, nameResolver, schemaOptions);
            }
            else
            {
                LogService.Error($"Provider {connection.Provider} not found");
                return null;
            }

        }

        public string GetConversion(string provider, string type)
        {
            return providers.ContainsKey(provider)
                ? providers[provider].Conversions != null && providers[provider].Conversions.ContainsKey(type)
                    ? providers[provider].Conversions[type]
                    : null
                : null;
        }

    }
}
