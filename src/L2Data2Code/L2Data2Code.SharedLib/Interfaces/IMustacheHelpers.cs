using System.Collections.Generic;

namespace L2Data2Code.SharedLib.Interfaces
{
    public interface IMustacheHelpers
    {
        void SetDictionary(IDictionary<string, object> keyValuePairs);
    }
}