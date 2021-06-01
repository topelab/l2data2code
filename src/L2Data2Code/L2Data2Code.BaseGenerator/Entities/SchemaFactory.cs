using L2Data2Code.SchemaReader.Fake;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Json;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.MySql;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SchemaReader.SqlServer;
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

        private static readonly Dictionary<string, ProviderDefinition> providers = new Dictionary<string, ProviderDefinition>()
        {
            { "System.Data.SqlClient", new ProviderDefinition{ Key = "sqlserver", Type = typeof(SqlServerSchemaReader) } },
            { "MySql.Data.MySqlClient", new ProviderDefinition{ Key = "mysql", Type = typeof(MySqlSchemaReader) } },
            { "System.Data.FakeClient", new ProviderDefinition{ Key = "fake", Type = typeof(FakeSchemaReader) } },
            { "System.Data.JsonClient", new ProviderDefinition {Key = "json", Type = typeof(JsonSchemaReader)} },
            { "Microsoft.Data.Sqlite", new ProviderDefinition { Key = "sqlite", Type = typeof(JsonSchemaReader),
                Conversions = new Dictionary<string, string>() {
                    { "decimal", "NUMERIC" } } } }
            //{ "Oracle.ManagedDataAccess.Client", typeof(OracleSchemaReader) },

        };

        public static string GetProviderDefinitionKey(string connectionStringKey)
        {
            var connection = new Connection(connectionStringKey);

            return providers.TryGetValue(connection.Provider, out ProviderDefinition providerDefinition) ? providerDefinition.Key : null;
        }

        public static ISchemaReader Create(string connectionStringKey, StringBuilderWriter summaryWriter, string connectionStringForObjectDescriptions = null)
        {
            Connection connection;
            Connection commentConnection;
            try
            {
                connection = new Connection(connectionStringKey);
                commentConnection = connectionStringForObjectDescriptions == null ? null : new Connection(connectionStringForObjectDescriptions);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
                return null;
            }

            if (commentConnection != null && providers.ContainsKey(commentConnection.Provider))
            {
                connectionStringForObjectDescriptions = commentConnection.ConnectionString;
            }

            if (providers.TryGetValue(connection.Provider, out ProviderDefinition providerDefinition))
            {
                return (ISchemaReader)Activator.CreateInstance(providerDefinition.Type, connection.ConnectionString, summaryWriter, connectionStringForObjectDescriptions);
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
