using L2Data2Code.BaseGenerator.Entities;
using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Interfaces
{
    public interface IReplacementCollectionFactory
    {
        Dictionary<string, object> Create(EntityTable table, CodeGeneratorDto options, Template template, Dictionary<string, object> internalVars);
    }
}