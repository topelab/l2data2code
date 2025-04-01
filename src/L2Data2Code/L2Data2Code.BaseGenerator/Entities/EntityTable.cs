using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using System.Collections.Generic;
using System.Linq;

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
        public bool MultiplePKColumns { get; private set; }
        public bool HasOnlyOnePKColumn { get; private set; }
        public bool IdentifiableById { get; private set; }
        public string Description { get; private set; }
        public string FieldDescriptor { get; private set; }
        public string FieldIdentity { get; private set; }
        public string FirstPK { get; private set; }
        public bool IsWeakEntity { get; private set; }
        public bool IsBigTable { get; private set; }
        public bool IsEnum => EnumValues.Count > 0;

        public List<EntityColumn> Columns = new();
        public List<Relation> OneToMany = new();
        public List<Relation> ManyToOne = new();
        public List<EntityIndex> Indexes = new();
        public List<EnumTableValue> EnumValues = new();
        public List<EntityColumn> FilterByColumns = [];

        public int NumeroCamposPK { get; set; }

        public EntityTable(Table table)
        {
            Name = table.CleanName;
            ClassName = table.ClassName;
            TableName = table.Name;
            TableType = table.Type;
            IsView = table.IsView;
            EnumValues = GetEnumTableValues(table);
            IsUpdatable = table.IsUpdatable;
            MultiplePKColumns = table.PK.Count() > 1;
            HasOnlyOnePKColumn = table.PK.Count() == 1;
            Description = table.Description;
            IsWeakEntity = table.IsWeakEntity;
            IsBigTable = table.IsBigTable;
            FieldDescriptor = table.DescriptionColumn;
            FieldIdentity = table.DescriptionId;

            CreateCampos(table);
            CreateIndexes(table);
            CreateOneToManyRelations(table);
            CreateManyToOneRelations(table);
        }

        private List<EnumTableValue> GetEnumTableValues(Table table)
        {
            var values = table.EnumValues.OrderBy(r => r.Id).ToList();
            return values.DistinctBy(r => r.Name).OrderBy(r => r.Id).ToList();
        }

        private void CreateCampos(Table table)
        {
            foreach (var column in table.Columns)
            {
                EntityColumn campo = new()
                {
                    Table = ClassName,
                    ColumnName = column.Name,
                    Name = column.PropertyName.Equals(table.ClassName)
                        ? (StringExtensions.CurrentLang == "es" ? $"Nombre{column.PropertyName}" : $"{column.PropertyName}Name")
                        : column.PropertyName,
                    Type = column.PropertyType + (column.IsNullable && Constants.NullableTypes.Contains(column.PropertyType) ? "?" : string.Empty),
                    IsNull = column.IsNullable,
                    Size = column.Precision,
                    Precision = column.Precision,
                    Scale = column.NumericScale,
                    IsNumeric = column.IsNumeric,
                    PrimaryKey = column.IsPK,
                    Description = column.Description,
                    PkOrder = column.PkOrder,
                    IsAutoIncrement = column.IsAutoIncrement,
                    IsComputed = column.IsComputed,
                    DefaultValue = column.DefaultValue,
                    IsFilter = column.IsFilter,
                    FilterType = column.FilterType,
                    FilterSpecification = column.FilterSpecification,
                };

                TrySetFilterType(campo);

                if (FieldDescriptor.IsEmpty() && (campo.Name == $"Nombre{table.ClassName}" || campo.Name == $"{table.ClassName}Name" || campo.Name == "Name" || campo.Name == "Nombre"))
                {
                    FieldDescriptor = campo.Name;
                }
                if (column.IsPK && HasOnlyOnePKColumn && campo.IsNumeric)
                {
                    campo.Name = "Id";
                    IdentifiableById = true;
                }
                if (column.IsPK && column.PkOrder == 1)
                {
                    FirstPK = campo.Name;
                    if (FieldIdentity.IsEmpty())
                    {
                        FieldIdentity = campo.Name;
                    }
                }

                Columns.Add(campo);
                NumeroCamposPK += campo.PrimaryKey ? 1 : 0;
            }
        }

        private static void TrySetFilterType(EntityColumn column)
        {
            if (column.IsFilter)
            {
                if (column.FilterType.IsEmpty())
                {
                    column.FilterType = column.Type switch
                    {
                        "DateTime" or "DateTime?" => "DateRange",
                        "TimeSpan" or "TimeSpan?" => "TimeRange",
                        "int" or "int?" or "long" or "long?" or "float" or "float?" or "double" or "double?" or "decimal" or "decimal?" => $"NumericRange",
                        "string" => "Text",
                        _ => throw new System.NotSupportedException($"Not supported filter type on Column {column.Table}.{column.Name} with type {column.Type}"),
                    };
                }

                if (column.FilterSubType == null && column.FilterType == "NumericRange")
                {
                    column.FilterSubType = $"<{column.Type}>";
                }
            }
        }

        private void CreateIndexes(Table table)
        {
            var columns = Columns.ToDictionary(k => k.ColumnName, k => k);

            foreach (var item in table.Indexes)
            {
                var fields = item.Columns.Select(c => new EntityIndexColumn(columns[c.Name], c.Order, c.IsDescending)).ToList();
                EntityIndex entityIndex = new(item.Name, item.IsUnique, fields);
                Indexes.Add(entityIndex);
            }
        }

        private void CreateOneToManyRelations(Table table)
        {
            var byWord = StringExtensions.CurrentLang == "es" ? "Por" : "By";

            OneToMany = table.OuterKeys.Select(r => new Relation
            {
                Table = r.ColumnReferenced.Table.ClassName,
                DbTable = r.ColumnReferenced.Table.Name,
                Column = r.ColumnReferenced.PropertyName,
                DbColumn = r.ColumnReferenced.Name,
                RelatedColumn = r.ColumnReferencing.PropertyName,
                DbRelatedColumn = r.ColumnReferencing.Name,
                CanBeNull = r.ColumnReferencing.IsNullable,
            }).ToList();

            foreach (var item in OneToMany)
            {
                var name = $"{item.Table}{byWord}{item.RelatedColumn}";
                var shortName = item.RelatedColumn.RemoveIdFromName();

                if (Columns.Any(r => r.ShortName == shortName))
                {
                    shortName = name;
                }

                EntityColumn campo = new()
                {
                    Table = ClassName,
                    Name = name,
                    ShortName = shortName,
                    Type = Constants.InternalTypes.ReferenceTo + item.Table,
                    IsNull = item.CanBeNull,
                    Size = 0,
                    Precision = 0,
                    Scale = 0,
                    PrimaryKey = false,
                    Description = $"ForeignKey from {item.RelatedColumn} to {item.Table}.{item.Column}",
                    IsForeignKey = true,
                    Join = item.Table,
                    DbJoin = item.DbTable,
                    FromField = item.RelatedColumn,
                    DbFromField = item.DbRelatedColumn,
                    ToField = item.Column,
                    DbToField = item.DbColumn,
                };
                Columns.Add(campo);
                SetRelated(item);
            }
        }

        private void SetRelated(Relation item)
        {
            var relatedColumn = Columns.Where(c => c.Name == item.RelatedColumn).FirstOrDefault();
            if (relatedColumn is not null)
            {
                relatedColumn.HasRelation = true;
                relatedColumn.Join = item.Table;
                relatedColumn.ToField = item.Column;
            }
        }

        private void CreateManyToOneRelations(Table table)
        {
            var withWord = StringExtensions.CurrentLang == "es" ? "Con" : "With";

            ManyToOne = table.InnerKeys.Select(r => new Relation
            {
                Table = r.ColumnReferencing.Table.ClassName,
                DbTable = r.ColumnReferencing.Table.Name,
                Column = r.ColumnReferencing.PropertyName,
                DbColumn = r.ColumnReferencing.Name,
                RelatedColumn = r.ColumnReferenced.PropertyName,
                DbRelatedColumn = r.ColumnReferenced.Name,
            }).ToList();

            foreach (var item in ManyToOne)
            {
                var name = $"{item.Table.ToPlural()}{withWord}{item.Column}";
                var shortName = item.Table.ToPlural();

                if (Columns.Any(r => r.ShortName == shortName))
                {
                    shortName = name;
                }

                EntityColumn campo = new()
                {
                    Table = ClassName,
                    Name = name,
                    ShortName = shortName,
                    Type = Constants.InternalTypes.Collection + item.Table,
                    IsNull = false,
                    Size = 0,
                    Precision = 0,
                    Scale = 0,
                    PrimaryKey = false,
                    Description = $"Collection of {item.Table.ToPlural()} with {item.Column} = this {item.RelatedColumn}",
                    IsCollection = true,
                    Join = item.Table,
                    DbJoin = item.DbTable,
                    FromField = item.RelatedColumn,
                    DbFromField = item.DbRelatedColumn,
                    ToField = item.Column,
                    DbToField = item.DbColumn,
                };
                Columns.Add(campo);
            }
        }
    }
}
