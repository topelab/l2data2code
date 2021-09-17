using System.Collections.Generic;

namespace L2Data2Code.SharedLib.Configuration
{
    public interface IBasicConfiguration<T> where T : class
    {
        T this[string key] { get; set; }

        int Count { get; }

        T FirstOrDefault();
        IEnumerable<string> GetKeys();
    }
}