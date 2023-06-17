namespace L2Data2Code.BaseGenerator.Entities
{
    public record EntityIndexColumn(EntityColumn EntityColumn, int Order, bool IsDescending)
    {
        public string ColumnName => EntityColumn.ColumnName;
        public string Name => EntityColumn.Name;
    }
}
