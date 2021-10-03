using L2Data2Code.SchemaReader.Fake;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Json;
using L2Data2Code.SchemaReader.MySql;
using L2Data2Code.SchemaReader.Object;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SchemaReader.SqlServer;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Helpers;
using System;
using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Entities
{
    public class SchemaFactory
    {
        internal class ProviderDefinition
        {
            public string Key { get; set; }
            public Type Type { get; set; }
            public Dictionary<string, string> Conversions { get; set; }
        }

        private static readonly Dictionary<string, ProviderDefinition> providers = new()
        {
            { "System.Data.SqlClient", new ProviderDefinition { Key = "sqlserver", Type = typeof(SqlServerSchemaReader) } },
            { "MySql.Data.MySqlClient", new ProviderDefinition { Key = "mysql", Type = typeof(MySqlSchemaReader) } },
            { "System.Data.FakeClient", new ProviderDefinition { Key = "fake", Type = typeof(FakeSchemaReader) } },
            { "System.Data.JsonClient", new ProviderDefinition { Key = "json", Type = typeof(JsonSchemaReader) } },
            {
                "Microsoft.Data.Sqlite",
                new ProviderDefinition
                {
                    Key = "sqlite",
                    Type = typeof(JsonSchemaReader),
                    Conversions = new Dictionary<string, string>() {
                    { "decimal", "NUMERIC" } }
                }
            },
            { "System.Data.ObjectClient", new ProviderDefinition { Key = "object", Type = typeof(ObjectSchemaReader) } },
            //{ "Oracle.ManagedDataAccess.Client", typeof(OracleSchemaReader) },

        };

        private static IBasicConfiguration<SchemaConfiguration> schemasConfiguration;

        public static string GetProviderDefinitionKey(string connectionStringKey)
        {
            var connection = new Connection(schemasConfiguration, connectionStringKey);
            return providers.TryGetValue(connection.Provider, out ProviderDefinition providerDefinition) ? providerDefinition.Key : null;
        }

        public static ISchemaReader Create(SchemaOptions schemaOptions)
        {
            Connection connection;
            Connection commentConnection;
            try
            {
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

            if (providers.TryGetValue(connection.Provider, out ProviderDefinition providerDefinition))
            {

                return (ISchemaReader)Activator.CreateInstance(providerDefinition.Type, schemaOptions);
            }
            else
            {
                LogService.Error($"Provider {connection.Provider} not found");
                return null;
            }

        }

        public static string GetConversion(string provider, string type)
        {
            return providers.ContainsKey(provider)
                ? (providers[provider].Conversions != null && providers[provider].Conversions.ContainsKey(type)
                    ? providers[provider].Conversions[type]
                    : null)
                : null;
        }

    }
}
