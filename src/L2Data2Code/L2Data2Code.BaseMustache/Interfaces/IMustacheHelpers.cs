using System.Collections.Generic;

namespace L2Data2Code.BaseMustache.Interfaces
{
    public interface IMustacheHelpers
    {
        void SetDictionary(IDictionary<string, object> keyValuePairs);
    }
}