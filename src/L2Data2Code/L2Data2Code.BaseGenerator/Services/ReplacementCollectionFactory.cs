using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SharedLib.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static L2Data2Code.BaseGenerator.Entities.Replacement;

namespace L2Data2Code.BaseGenerator.Services
{
    public class ReplacementCollectionFactory : IReplacementCollectionFactory
    {
        private readonly ISchemaService schemaService;
        private readonly ISchemaFactory schemaFactory;

        public ReplacementCollectionFactory(ISchemaService schemaService, ISchemaFactory schemaFactory)
        {
            this.schemaService = schemaService ?? throw new ArgumentNullException(nameof(schemaService));
            this.schemaFactory = schemaFactory ?? throw new ArgumentNullException(nameof(schemaFactory));
        }

        public Dictionary<string, object> Create(EntityTable table, CodeGeneratorDto options, Template template, Dictionary<string, object> internalVars)
        {
            var (ConnectionString, Provider) = schemaService.GetConnectionString(options.CreatedFromSchemaName);

            var normalizeNames = schemaService.NormalizedNames(options.CreatedFromSchemaName);

            Entity entity = new()
            {
                Name = table.ClassName,
                Type = table.TableType,
                FieldDescriptor = table.FieldDescriptor,
                FieldIdentity = table.FieldIdentity,
                FirstPK = table.FirstPK,
                UseSpanish = schemaService.GetLang(options.CreatedFromSchemaName).Equals("es", StringComparison.CurrentCultureIgnoreCase),
                MultiplePKColumns = table.MultiplePKColumns,
                HasOnlyOnePKColumn = table.HasOnlyOnePKColumn,
                IdentifiableById = table.IdentifiableById,
            };

            var properties =
                table.Columns.Select(
                    (column, index, isFirst, isLast) =>
                    {
                        var name = column.Name;
                        var type = DecodeCSharpType(column.Type);
                        Property property = new()
                        {
                            Entity = entity,
                            Table = table.TableName,
                            Name = name,
                            ShortName = column.ShortName,
                            Nullable = column.IsNull,
                            PrimaryKey = column.PrimaryKey,
                            IsFirst = isFirst,
                            IsLast = isLast,
                            DefaultValue = column.GetDefaultValue(),
                            InitialValue = column.GetInitialValue(),
                            HasDefaultValue = column.HasDefaultValue,
                            Type = type,
                            OverrideDbType = schemaFactory.GetConversion(Provider, type),
                            Description = string.IsNullOrWhiteSpace(column.Description) ? null : column.Description.ReplaceEndOfLine(),
                            IsCollection = column.IsCollection,
                            IsForeignKey = column.IsForeignKey,
                            ColumnName = column.ColumnName,
                            ColumnNameOrName = normalizeNames ? name : column.ColumnName,
                            IsAutoIncrement = column.IsAutoIncrement,
                            IsComputed = column.IsComputed,
                            PkOrder = column.PkOrder,
                            MultiplePKColumns = table.MultiplePKColumns,
                            Precision = column.Precision,
                            Scale = column.Scale,
                            IsNumeric = column.IsNumeric,
                            IsString = column.Type.StartsWith("string"),
                            IsDateOrTime = column.Type.StartsWith("DateTime") || column.Type.StartsWith("TimeSpan"),
                            Join = column.Join,
                            FromField = column.FromField,
                            ToField = column.ToField,
                            ToFieldDescriptor = column.ToFieldDescriptor,
                            DbJoin = column.DbJoin,
                            DbFromField = column.DbFromField,
                            DbToField = column.DbToField,
                            HasRelation = column.HasRelation,
                            IsFilter = column.IsFilter,
                            FilterType = column.FilterType,
                        };
                        return property;
                    }).ToArray();

            Replacement replacement = new()
            {
                Template = template.Name,
                Entity = entity,
                Indexes = table.Indexes.ToArray(),
                EnumValues = table.EnumValues.ToArray(),
                IsView = table.IsView,
                IsUpdatable = table.IsUpdatable,
                Description = string.IsNullOrWhiteSpace(table.Description) ? null : table.Description.ReplaceEndOfLine(),
                ConnectionString = ConnectionString,
                DataProvider = Provider,
                Module = template.Module,
                Area = template.Area,
                Company = template.Company,
                TableName = table.TableName,
                TableNameOrEntity = normalizeNames ? entity.Name : table.TableName,
                GenerateReferences = options.GenerateReferenced,
                IgnoreColumns = template.IgnoreColumns == null ? Array.Empty<string>() : template.IgnoreColumns.Replace(" ", "").Split(Path.PathSeparator),
                UnfilteredColumns = properties,
                GenerateBase = false,
                Vars = internalVars,
                CanCreateDB = schemaService.CanCreateDB(options.CreatedFromSchemaName),
                IsBigTable = table.IsBigTable,
            };

            var filteredColumns = properties
                    .Where(p => !replacement.IgnoreColumns.Contains(p.Name, IgnoreCaseComparer.Instance))
                    .Where(p => !(replacement.IgnoreColumns.Contains(Constants.ID) && p.IsEntityId()));

            replacement.AllColumns = filteredColumns
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

            replacement.Columns = filteredColumns
                    .Where(p => !p.IsForeignKey && !p.IsCollection)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

            replacement.PersistedColumns = filteredColumns
                    .Where(p => !p.IsForeignKey && !p.IsCollection && !p.IsComputed && !p.PrimaryKey)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

            replacement.ForeignKeyColumns = filteredColumns
                    .Where(p => p.IsForeignKey)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

            replacement.DistinctForeignKeyColumnsByType = filteredColumns
                    .Where(p => p.IsForeignKey)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .Where(p => p.Entity.Name != p.Type)
                    .DistinctBy(p => p.Type)
                    .ToArray();

            replacement.NotRelatedColumns = filteredColumns
                    .Where(p => !p.IsForeignKey && !p.IsCollection && !p.PrimaryKey && !p.HasRelation)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

            replacement.Collections = filteredColumns
                    .Where(p => p.IsCollection)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

            replacement.NotPrimaryKeyColumns = filteredColumns
                    .Where(p => !p.PrimaryKey)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

            replacement.PrimaryKeys = filteredColumns
                    .Where(p => !p.IsForeignKey && !p.IsCollection)
                    .Where(p => p.PrimaryKey)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

            replacement.NotPrimaryKeys = filteredColumns
                    .Where(p => !p.IsForeignKey && !p.IsCollection && !p.PrimaryKey)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

            replacement.FilterByColumns = filteredColumns
                    .Where(p => p.IsFilter)
                    .Select((param, index, isFirst, isLast) => param.Clone(isFirst, isLast))
                    .ToArray();

            replacement.HasCollections = filteredColumns.Any(p => p.IsCollection);
            replacement.HasForeignKeys = filteredColumns.Any(p => p.IsForeignKey);
            replacement.HasNotPrimaryKeyColumns = replacement.NotPrimaryKeys.Any();
            replacement.HasPrimaryKeyColumns = replacement.PrimaryKeys.Any();
            replacement.MultiplePKColumns = replacement.PrimaryKeys.Length > 1;

            var primaryKeys = properties.Where(p => p.PrimaryKey);
            replacement.IsWeakEntity = table.IsWeakEntity || primaryKeys.Count() != 1 || primaryKeys.None(p => p.IsEntityId());

            return GetDictionaryDataFromReplacement(replacement);
        }

        private static string DecodeCSharpType(string type) =>
            type.StartsWith(Constants.InternalTypes.Collection) || type.StartsWith(Constants.InternalTypes.ReferenceTo) ? type[1..] : type;

        private static Dictionary<string, object> GetDictionaryDataFromReplacement(Replacement replacementData)
        {
            var data = new Dictionary<string, object>();

            var properties = replacementData.GetType().GetProperties();
            foreach (var property in properties.Where(p => p.Name != "Item"))
            {
                data[property.Name] = property.GetValue(replacementData);
            }

            foreach (var key in replacementData.Vars.Keys)
            {
                data[key] = replacementData.Vars[key];
            }

            return data;
        }
    }
}
