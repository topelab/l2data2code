using L2Data2Code.BaseGenerator.Entities;
using System;
using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Interfaces
{
    public interface ICodeGeneratorService
    {
        void Initialize(CodeGeneratorDto options, TemplateLibrary library, Dictionary<string, object> vars = null);
        void ProcessTables(Action<string> onTableProcessed = null, Dictionary<string, string> alternativeDictionary = null);
        Dictionary<string, object> GetVars();
        string GetSolutionType();
    }
}