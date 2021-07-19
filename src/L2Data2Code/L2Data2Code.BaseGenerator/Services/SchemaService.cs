using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Helpers;
using NLog;
using System;
using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Services
{
    public class SchemaService : ISchemaService
    {
        private readonly IMustacheRenderizer mustacheRenderizer;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, Tables> connectionsStringsReady;

        public SchemaService(IMustacheRenderizer mustacheRenderizer)
        {
            this.mustacheRenderizer = mustacheRenderizer;
            connectionsStringsReady = new();
        }

        public Tables Read(CodeGeneratorDto options, Dictionary<string, string> alternativeDescriptions = null)
        {
            var salida = new StringBuilderWriter();

            try
            {
                mustacheRenderizer.SetIsoLanguaje(Config.GetLang(options.CreatedFromConnectionStringName));
                var tableNameResolver = new NameResolver(options.CreatedFromConnectionStringName);

                if (!connectionsStringsReady.ContainsKey(options.ConnectionStringName))
                {
                    var schemaReader = SchemaFactory.Create(new SchemaOptions(options.ConnectionStringName, salida, options.ConnectionNameForDescriptions));
                    if (schemaReader == null)
                    {
                        throw new Exception($"Cannot create schema reader. Reason: {LogService.LastError}");
                    }

                    var tables = schemaReader.ReadSchema(new SchemaReaderOptions(Config.ShouldRemoveWord1(options.ConnectionStringName), alternativeDescriptions, tableNameResolver));

                    if (schemaReader.HasErrorMessage())
                    {
                        logger.Error($"\"{salida.OutputStringBuilder}\"");
                    }

                    connectionsStringsReady.Add(options.ConnectionStringName, tables);
                }

                return connectionsStringsReady[options.ConnectionStringName];
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
