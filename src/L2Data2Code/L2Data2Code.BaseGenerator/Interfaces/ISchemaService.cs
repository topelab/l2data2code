using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.SchemaReader.Schema;
using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Interfaces
{
    public interface ISchemaService
    {
        Tables Read(CodeGeneratorDto options, Dictionary<string, string> alternativeDescriptions = null);
    }
}