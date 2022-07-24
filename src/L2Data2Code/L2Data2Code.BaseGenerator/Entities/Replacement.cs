using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace L2Data2Code.BaseGenerator.Entities
{
    /// <summary>
    /// Class that represent de object used for replacement in a mustache context
    /// </summary>
    public class Replacement : IDictionary<string, object>
    {
        private IEnumerable<Property> FilteredColumns => UnfilteredColumns
                    .Where(p => !IgnoreColumns.Contains(p.Name, IgnoreCaseComparer.Instance))
                    .Where(p => !(IgnoreColumns.Contains(Constants.ID) && p.IsEntityId()));

        public string Template { get; set; }
        public Entity Entity { get; set; }
        public string Module { get; set; }
        public string Area { get; set; }
        public string Company { get; set; }
        public string TableName { get; set; }
        public string TableNameOrEntity { get; set; }
        public string[] IgnoreColumns { get; set; }
        public Property[] UnfilteredColumns { get; set; }
        public bool GenerateBase { get; set; }
        public bool GenerateReferences { get; set; }
        public bool IsView { get; set; }
        public bool IsUpdatable { get; set; }
        public string Description { get; set; }
        public string ConnectionString { get; set; }
        public string DataProvider { get; set; }
        public Dictionary<string, object> Vars { get; set; } = new Dictionary<string, object>();

        public Property[] AllColumns => FilteredColumns
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

        public Property[] Columns => FilteredColumns
                    .Where(p => !p.IsForeignKey && !p.IsCollection)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

        public Property[] PersistedColumns => FilteredColumns
                    .Where(p => !p.IsForeignKey && !p.IsCollection && !p.IsComputed && !p.PrimaryKey)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();
        public Property[] ForeignKeyColumns => FilteredColumns
                    .Where(p => p.IsForeignKey)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

        public Property[] Collections => FilteredColumns
                    .Where(p => p.IsCollection)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

        public bool HasCollections => FilteredColumns.Any(p => p.IsCollection);

        public bool HasForeignKeys => FilteredColumns.Any(p => p.IsForeignKey);

        public Property[] NotPrimaryKeyColumns => FilteredColumns
                    .Where(p => !p.PrimaryKey)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

        public Property[] PrimaryKeys => FilteredColumns
                    .Where(p => !p.IsForeignKey && !p.IsCollection)
                    .Where(p => p.PrimaryKey)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

        public Property[] NotPrimaryKeys => FilteredColumns
                    .Where(p => !p.IsForeignKey && !p.IsCollection && !p.PrimaryKey)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

        public bool HasNotPrimaryKeyColumns => NotPrimaryKeys.Any();
        public bool HasPrimaryKeyColumns => PrimaryKeys.Any();
        public bool MultiplePKColumns => PrimaryKeys.Length > 1;
        public bool IsWeakEntity
        {
            get
            {
                var keys = UnfilteredColumns.Where(p => p.PrimaryKey);
                return keys.Count() != 1 || keys.None(p => p.IsEntityId());
            }
        }

        public ICollection<string> Keys => ((IDictionary<string, object>)Vars).Keys;

        public ICollection<object> Values => ((IDictionary<string, object>)Vars).Values;

        public int Count => ((IDictionary<string, object>)Vars).Count;

        public bool IsReadOnly => ((IDictionary<string, object>)Vars).IsReadOnly;

        public bool CanCreateDB { get; internal set; }

        public object this[string key] { get => ((IDictionary<string, object>)Vars)[key]; set => ((IDictionary<string, object>)Vars)[key] = value; }

        private class IgnoreCaseComparer : IEqualityComparer<string>
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
                            if (_instance == null)
                            {
                                _instance = new IgnoreCaseComparer();
                            }
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

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, object>)Vars).ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            ((IDictionary<string, object>)Vars).Add(key, value);
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, object>)Vars).Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return ((IDictionary<string, object>)Vars).TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            ((IDictionary<string, object>)Vars).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<string, object>)Vars).Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return ((IDictionary<string, object>)Vars).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((IDictionary<string, object>)Vars).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return ((IDictionary<string, object>)Vars).Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return ((IDictionary<string, object>)Vars).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, object>)Vars).GetEnumerator();
        }
    }
}
