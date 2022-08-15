using L2Data2Code.SharedLib.Extensions;
using System;

namespace L2Data2Code.BaseGenerator.Entities
{
    /// <summary>
    /// Represents a property for an entity that could be a column, an object property, ect.
    /// </summary>
    public class Property : ICloneable
    {
        public Entity Entity { get; set; }
        public string Name { get; set; }
        public string IdOrName => IsEntityId() ? Constants.ID : Name;
        public string Table { get; set; }
        public bool Nullable { get; set; }
        public bool PrimaryKey { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsCollection { get; set; }
        public string DefaultValue { get; set; }
        public string Type { get; set; }
        public string OverrideDbType { get; set; }
        public bool DbTypeOverrided => OverrideDbType.NotEmpty();
        public string NullableType => Type.Equals("string") || Type.EndsWith("?") || Type.StartsWith("byte[") ? Type : $"{Type}?";
        public bool HasMaxLength => Type.Equals("string");
        public string Description { get; set; }
        public string ColumnName { get; set; }
        public string ColumnNameOrName { get; set; }
        public int PkOrder { get; set; }
        public bool IsAutoIncrement { get; set; }
        public bool IsComputed { get; set; }
        public bool MultiplePKColumns { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public bool IsNumeric { get; set; }
        public bool IsString { get; set; }
        public bool IsDateOrTime { get; set; }
        public string Join { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
        public string DbJoin { get; set; }
        public string DbFromField { get; set; }
        public string DbToField { get; set; }
        public string FirstSample
        {
            get
            {
                _sampleValue = 1;
                return GetSampleValue(Type);
            }
        }
        public string Sample => GetSampleValue(Type);
        public string NextSample => GetSampleValue(Type, true);

        private static int _sampleValue = 1;
        private static string GetSampleValue(string type, bool anotherOne = false)
        {
            _sampleValue += anotherOne ? 1 : 0;

            return type switch
            {
                "string" => $"\"SAMPLE {_sampleValue:0000}\"",
                "int" => _sampleValue.ToString(),
                "long" => _sampleValue.ToString(),
                "decimal" => _sampleValue.ToString(),
                "float" => _sampleValue.ToString(),
                "double" => _sampleValue.ToString(),
                "DateTime" => $"DateTime.Today.AddDays({_sampleValue - 1})",
                "TimeSpan" => $"TimeSpan.FromMinutes({_sampleValue})",
                _ => "0"
            };
        }

        public bool IsEntityId()
        {
            return
                  Name != null
                &&
                  (Name == Constants.ID
                  || Name.Equals(Constants.ID + Table, StringComparison.CurrentCultureIgnoreCase)
                  || Name.Equals(Table + Constants.ID, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool IsNameDifferentToColumnName { get => !ColumnName.Equals(Name); }

        public object Clone()
        {
            return Clone(IsFirst, IsLast);
        }

        public Property Clone(bool isFirst, bool isLast)
        {
            return new Property
            {
                Entity = Entity,
                Table = Table,
                Name = Name,
                Nullable = Nullable,
                PrimaryKey = PrimaryKey,
                DefaultValue = DefaultValue,
                Type = Type,
                OverrideDbType = OverrideDbType,
                Description = Description,
                IsFirst = isFirst,
                IsLast = isLast,
                IsCollection = IsCollection,
                IsForeignKey = IsForeignKey,
                ColumnName = ColumnName,
                ColumnNameOrName = ColumnNameOrName,
                PkOrder = PkOrder,
                IsAutoIncrement = IsAutoIncrement,
                IsComputed = IsComputed,
                MultiplePKColumns = MultiplePKColumns,
                Precision = Precision,
                Scale = Scale,
                IsNumeric = IsNumeric,
                IsDateOrTime = IsDateOrTime,
                IsString = IsString,
                Join = Join,
                DbJoin = DbJoin,
                FromField = FromField,
                DbFromField = DbFromField,
                ToField = ToField,
                DbToField = DbToField,
            };
        }

        public Property Clone(bool isFirst, bool isLast, Action<Property> modifyAfterClone)
        {
            var clon = Clone(isFirst, isLast);
            modifyAfterClone(clon);
            return clon;
        }
    }

}
