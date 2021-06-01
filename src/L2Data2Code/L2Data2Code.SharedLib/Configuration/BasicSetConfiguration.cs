using L2Data2Code.SharedLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Data2Code.SharedLib.Configuration
{
    public class BasicSetConfiguration
    {
        private readonly string _list;
        private readonly HashSet<string> _valueCollection;

        public BasicSetConfiguration(string list)
        {
            _list = list;
            _valueCollection = ConfigHelper.Config[list].ToHashSet();
        }
        public IEnumerable<string> GetKeys()
        {
            return _valueCollection.ToList();
        }

        public string FirstOrDefault()
        {
            return _valueCollection.FirstOrDefault();
        }

        public string this[string key]
        {
            get => _valueCollection.Contains(key) ? key : null;
            set
            {
                if (!_valueCollection.Contains(key))
                    _valueCollection.Add(key);
            }
        }
    }
}
