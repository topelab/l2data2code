using L2Data2Code.SchemaReader.Schema;
using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Entities
{
    public class EntityTable
    {
        public string Name { get; internal set; }
        public string ClassName { get; internal set; }
        public string TableName { get; internal set; }
        public string TableType { get; internal set; }
        public bool IsView { get; internal set; }
        public bool IsUpdatable { get; internal set; }
        public bool MultiplePKColumns { get; internal set; }
        public bool HasOnlyOnePKColumn { get; internal set; }
        public bool IdentifiableById { get; internal set; }
        public string Description { get; internal set; }
        public string FieldDescriptor { get; internal set; }
        public string FieldIdentity { get; internal set; }
        public string FirstPK { get; internal set; }
        public bool IsWeakEntity { get; internal set; }
        public bool IsBigTable { get; internal set; }
        public bool IsEnum => EnumValues.Count > 0;
        public int NumeroCamposPK { get; set; }

        public List<EntityColumn> Columns = [];
        public List<Relation> OneToMany = [];
        public List<Relation> ManyToOne = [];
        public List<EntityIndex> Indexes = [];
        public List<EnumTableValue> EnumValues = [];
        public List<EntityColumn> FilterByColumns = [];
    }
}
