using System;
using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Entities
{
    public partial class Replacement
    {
        public class IgnoreCaseComparer : IEqualityComparer<string>
        {
            private IgnoreCaseComparer() { }
            private static IgnoreCaseComparer _instance = null;
            private static readonly object _syncRoot = new();

            public static IEqualityComparer<string> Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        lock (_syncRoot)
                        {
                            _instance ??= new IgnoreCaseComparer();
                        }
                    }

                    return _instance;
                }
            }

            public bool Equals(string x, string y)
            {
                return x.Equals(y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
