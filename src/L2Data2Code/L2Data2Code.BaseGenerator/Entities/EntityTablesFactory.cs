using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace L2Data2Code.BaseGenerator.Entities
{
    internal class EntityTablesFactory : IEntityTablesFactory
    {
        public Dictionary<string, EntityTable> Create(IEnumerable<Table> tables)
        {
            Dictionary<string, EntityTable> result = new();

            foreach (var table in tables)
            {
                var entityTable = CreateEntityTable(table);
                result.Add(entityTable.TableName, entityTable);
            }

            foreach (var table in result.Values)
            {
                SetFilterSpecification(result, table);
                SetFieldDescriptors(result, table);
                SetRelationsTypes(result, table);
            }

            return result;
        }

        private EntityTable CreateEntityTable(Table table)
        {
            EntityTable entityTable = new()
            {
                Name = table.CleanName,
                ClassName = table.ClassName,
                TableName = table.Name,
                TableType = table.Type,
                IsView = table.IsView,
                EnumValues = GetEnumTableValues(table),
                IsUpdatable = table.IsUpdatable,
                MultiplePKColumns = table.PK.Count() > 1,
                HasOnlyOnePKColumn = table.PK.Count() == 1,
                Description = table.Description,
                IsWeakEntity = table.IsWeakEntity,
                IsBigTable = table.IsBigTable,
                FieldDescriptor = table.DescriptionColumn,
                FieldIdentity = table.DescriptionId
            };

            CreateCampos(table, entityTable);
            CreateIndexes(table, entityTable);
            CreateOneToManyRelations(table, entityTable);
            CreateManyToOneRelations(table, entityTable);


            return entityTable;
        }

        private List<EnumTableValue> GetEnumTableValues(Table table)
        {
            var values = table.EnumValues.OrderBy(r => r.Id).ToList();
            return values.DistinctBy(r => r.Name).OrderBy(r => r.Id).ToList();
        }

        private void CreateCampos(Table table, EntityTable entityTable)
        {
            foreach (var column in table.Columns)
            {
                EntityColumn campo = new()
                {
                    Table = entityTable.ClassName,
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

                if (entityTable.FieldDescriptor.IsEmpty() && (campo.Name == $"Nombre{table.ClassName}" || campo.Name == $"{table.ClassName}Name" || campo.Name == "Name" || campo.Name == "Nombre"))
                {
                    entityTable.FieldDescriptor = campo.Name;
                }
                if (column.IsPK && entityTable.HasOnlyOnePKColumn && campo.IsNumeric)
                {
                    campo.Name = "Id";
                    entityTable.IdentifiableById = true;
                }
                if (column.IsPK && column.PkOrder == 1)
                {
                    entityTable.FirstPK = campo.Name;
                    if (entityTable.FieldIdentity.IsEmpty())
                    {
                        entityTable.FieldIdentity = campo.Name;
                    }
                }

                entityTable.Columns.Add(campo);
                entityTable.NumeroCamposPK += campo.PrimaryKey ? 1 : 0;
            }
        }

        private void TrySetFilterType(EntityColumn column)
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

                column.FilterPrimitive = column.FilterType switch
                {
                    "Text" => "string",
                    "Combo" => "DescriptionDTO",
                    _ => string.Concat(column.FilterType, column.FilterSubType),
                };
            }
        }

        private void CreateIndexes(Table table, EntityTable entityTable)
        {
            var columns = entityTable.Columns.ToDictionary(k => k.ColumnName, k => k);

            foreach (var item in table.Indexes)
            {
                var fields = item.Columns.Select(c => new EntityIndexColumn(columns[c.Name], c.Order, c.IsDescending)).ToList();
                EntityIndex entityIndex = new(item.Name, item.IsUnique, fields);
                entityTable.Indexes.Add(entityIndex);
            }
        }

        private void CreateOneToManyRelations(Table table, EntityTable entityTable)
        {
            var byWord = StringExtensions.CurrentLang == "es" ? "Por" : "By";

            entityTable.OneToMany = table.OuterKeys.Select(r => new Relation
            {
                Table = r.ColumnReferenced.Table.ClassName,
                DbTable = r.ColumnReferenced.Table.Name,
                Column = r.ColumnReferenced.PropertyName,
                DbColumn = r.ColumnReferenced.Name,
                RelatedColumn = r.ColumnReferencing.PropertyName,
                DbRelatedColumn = r.ColumnReferencing.Name,
                CanBeNull = r.ColumnReferencing.IsNullable,
            }).ToList();

            foreach (var item in entityTable.OneToMany)
            {
                var name = $"{item.Table}{byWord}{item.RelatedColumn}";
                var shortName = item.RelatedColumn.RemoveIdFromName();

                if (entityTable.Columns.Any(r => r.ShortName == shortName))
                {
                    shortName = name;
                }

                EntityColumn campo = new()
                {
                    Table = entityTable.ClassName,
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
                entityTable.Columns.Add(campo);
                SetRelated(item, entityTable, campo);
            }
        }

        private void SetRelated(Relation item, EntityTable entityTable, EntityColumn campo)
        {
            var relatedColumn = entityTable.Columns.Where(c => c.Name == item.RelatedColumn).FirstOrDefault();
            if (relatedColumn is not null)
            {
                relatedColumn.HasRelation = true;
                relatedColumn.Join = item.Table;
                relatedColumn.ToField = item.Column;
                campo.ToFieldType = relatedColumn.Type;
            }
        }

        private void CreateManyToOneRelations(Table table, EntityTable entityTable)
        {
            var withWord = StringExtensions.CurrentLang == "es" ? "Con" : "With";

            entityTable.ManyToOne = table.InnerKeys.Select(r => new Relation
            {
                Table = r.ColumnReferencing.Table.ClassName,
                DbTable = r.ColumnReferencing.Table.Name,
                Column = r.ColumnReferencing.PropertyName,
                DbColumn = r.ColumnReferencing.Name,
                RelatedColumn = r.ColumnReferenced.PropertyName,
                DbRelatedColumn = r.ColumnReferenced.Name,
            }).ToList();

            foreach (var item in entityTable.ManyToOne)
            {
                var name = $"{item.Table.ToPlural()}{withWord}{item.Column}";
                var shortName = item.Table.ToPlural();

                if (entityTable.Columns.Any(r => r.ShortName == shortName))
                {
                    shortName = name;
                }

                EntityColumn campo = new()
                {
                    Table = entityTable.ClassName,
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
                    // ToFieldType = entityTable.Columns.FirstOrDefault(c => c.ColumnName == item.Column)?.Type,
                    DbToField = item.DbColumn,
                };
                entityTable.Columns.Add(campo);
            }
        }

        private void SetFilterSpecification(Dictionary<string, EntityTable> entityTables, EntityTable tabla)
        {
            foreach (var column in tabla.Columns.Where(c => c.FilterSpecification.NotEmpty()))
            {
                var referenceTable = entityTables.Values.FirstOrDefault(t => t.TableName.Equals(column.FilterSpecification, StringComparison.OrdinalIgnoreCase));
                if (referenceTable != null)
                {
                    column.HasRelation = true;
                    column.FromField = column.Name;
                    column.Join = referenceTable.ClassName;
                    column.ShortName = referenceTable.ClassName;
                    column.ToField = referenceTable.FieldIdentity;
                    column.ToFieldDescriptor = referenceTable.FieldDescriptor;
                    column.ToFieldType = referenceTable.Columns.FirstOrDefault(c => c.Name == column.ToField)?.Type;
                }
            }
        }

        private void SetFieldDescriptors(Dictionary<string, EntityTable> entityTables, EntityTable tabla)
        {
            foreach (var column in tabla.Columns.Where(c => c.HasRelation && c.ToFieldDescriptor.IsEmpty()))
            {
                var referenceTable = entityTables.Values.FirstOrDefault(t => t.ClassName.Equals(column.Join, StringComparison.OrdinalIgnoreCase));
                if (referenceTable != null)
                {
                    column.ToFieldDescriptor = referenceTable.FieldDescriptor;
                }
            }
        }

        private void SetRelationsTypes(Dictionary<string, EntityTable> entityTables, EntityTable tabla)
        {
            foreach (var column in tabla.Columns.Where(c => c.IsCollection))
            {
                var referenceTable = entityTables.Values.FirstOrDefault(t => t.ClassName.Equals(column.Join, StringComparison.OrdinalIgnoreCase));
                if (referenceTable != null)
                {
                    column.ToFieldType = referenceTable.Columns.FirstOrDefault(c => c.Name == column.ToField)?.Type;
                }
            }
        }
    }
}
