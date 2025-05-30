using L2Data2Code.SchemaReader.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace L2Data2Code.BaseGenerator.Entities
{
    /// <summary>
    /// Class that represent de object used for replacement in a mustache context
    /// </summary>
    public partial class Replacement : IDictionary<string, object>
    {
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
        public Property[] AllColumns { get; set; }
        public Property[] Columns { get; set; }
        public Property[] PersistedColumns { get; set; }
        public Property[] ForeignKeyColumns { get; set; }
        public Property[] DistinctForeignKeyColumnsByType { get; set; }
        public Property[] Collections { get; set; }
        public Property[] NotPrimaryKeyColumns { get; set; }
        public Property[] PrimaryKeys { get; set; }
        public Property[] NotPrimaryKeys { get; set; }
        public Property[] NotRelatedColumns { get; set; }
        public Property[] FilterByColumns { get; set; }
        public Property[] DistinctFilterByColumns { get; set; }
        public Property[] ManualRelatedColumns { get; set; }
        public EntityIndex[] Indexes { get; set; }
        public EnumTableValue[] EnumValues { get; set; }
        public bool HasCollections { get; set; }
        public bool HasForeignKeys { get; set; }
        public bool HasManualRelatedColumns { get; set; }
        public bool HasNotPrimaryKeyColumns { get; set; }
        public bool HasPrimaryKeyColumns { get; set; }
        public bool MultiplePKColumns { get; set; }
        public bool IsWeakEntity { get; set; }
        public bool IsBigTable { get; set; }
        public bool HasValues => EnumValues?.Any() ?? false;
        public bool IsBig => IsBigTable && !IsWeakEntity;
        public bool IsSmall => !IsBigTable && !IsWeakEntity;

        public ICollection<string> Keys => ((IDictionary<string, object>)Vars).Keys;

        public ICollection<object> Values => ((IDictionary<string, object>)Vars).Values;

        public int Count => ((IDictionary<string, object>)Vars).Count;

        public bool IsReadOnly => ((IDictionary<string, object>)Vars).IsReadOnly;

        public bool CanCreateDB { get; internal set; }

        public object this[string key] { get => ((IDictionary<string, object>)Vars)[key]; set => ((IDictionary<string, object>)Vars)[key] = value; }

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
