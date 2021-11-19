using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using NLog;
using System;
using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Services
{
    public class SchemaService : ISchemaService
    {
        private readonly ILogger logger;
        private readonly Dictionary<string, Tables> schemaNamesCached;

        public SchemaService(ILogger logger)
        {
            this.logger = logger;
            schemaNamesCached = new();
        }

        public Tables Read(CodeGeneratorDto options, Dictionary<string, string> alternativeDescriptions = null)
        {
            StringBuilderWriter salida = new();

            try
            {
                StringExtensions.CurrentLang = Config.GetLang(options.CreatedFromSchemaName);
                NameResolver tableNameResolver = new(options.CreatedFromSchemaName);

                if (!schemaNamesCached.ContainsKey(options.SchemaName))
                {
                    var schemaReader = SchemaFactory.Create(new SchemaOptions(options.SchemasConfiguration, options.SchemaName, salida, options.DescriptionsSchemaName));
                    if (schemaReader == null)
                    {
                        throw new Exception($"Cannot create schema reader. Reason: {LogService.LastError}");
                    }

                    var tables = schemaReader.ReadSchema(new SchemaReaderOptions(Config.ShouldRemoveWord1(options.SchemaName), alternativeDescriptions, tableNameResolver));

                    if (schemaReader.HasErrorMessage())
                    {
                        logger.Error($"\"{salida.OutputStringBuilder}\"");
                    }

                    schemaNamesCached.Add(options.SchemaName, tables);
                }

                return schemaNamesCached[options.SchemaName];
            }
            catch (Exception ex)
            {
                logger.Error($"Error reading schema: {ex.Message}");
                if (salida.ContainsErrorMessage)
                    logger.Error($"\n{salida.OutputStringBuilder}");

                throw;
            }

        }
    }
}
