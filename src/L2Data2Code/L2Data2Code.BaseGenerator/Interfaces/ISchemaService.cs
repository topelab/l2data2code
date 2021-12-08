using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.SchemaReader.Schema;
using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Interfaces
{
    public interface ISchemaService
    {
        bool CanCreateDB(string schemaName);
        (string ConnectionString, string Provider) GetConnectionString(string schemaName);
        string GetLang(string schemaName);
        Dictionary<string, string> GetSchemaDictionaryFromFile(string schemaName);
        bool NormalizedNames(string schemaName);
        Tables Read(CodeGeneratorDto options, Dictionary<string, string> alternativeDescriptions = null);
        bool ShouldRemoveWord1(string schemaName);
    }
}