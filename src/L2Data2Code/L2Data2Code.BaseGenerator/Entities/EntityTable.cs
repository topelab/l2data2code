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
        public bool IsView { get; internal set; }
        public bool IsUpdatable { get; internal set; }
        public bool MultiplePKColumns { get; private set; }
        public string Description { get; private set; }

        public List<EntityColumn> Columns = new();
        public List<Relation> OneToMany = new();
        public List<Relation> ManyToOne = new();

        public int NumeroCamposPK { get; set; }

        public EntityTable(Table table)
        {
            Name = table.CleanName;
            ClassName = table.ClassName;
            TableName = table.Name;
            IsView = table.IsView;
            IsUpdatable = table.IsUpdatable;
            MultiplePKColumns = table.PK.Count() > 1;
            Description = table.Description;

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
                };
                Columns.Add(campo);
                NumeroCamposPK += campo.PrimaryKey ? 1 : 0;

            }

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

            var byWord = StringExtensions.CurrentLang == "es" ? "Por" : "By";
            var withWord = StringExtensions.CurrentLang == "es" ? "Con" : "With";

            foreach (var item in OneToMany)
            {
                EntityColumn campo = new()
                {
                    Table = ClassName,
                    Name = $"{item.Table}{byWord}{item.RelatedColumn}",
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
            }

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
                EntityColumn campo = new()
                {
                    Table = ClassName,
                    Name = $"{item.Table.ToPlural()}{withWord}{item.RelatedColumn}",
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
