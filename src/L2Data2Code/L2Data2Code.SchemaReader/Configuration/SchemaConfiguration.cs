namespace L2Data2Code.SchemaReader.Configuration
{
    public class SchemaConfiguration
    {
        public string ConnectionString { get; set; }
        public string Provider { get; set; }
        public string TableNameLanguage { get; set; } = "en";
        public bool RemoveFirstWordOnColumnNames { get; set; } = false;
        public string DescriptionsFile { get; set; }
        public string RenameTables { get; set; }
        public string RenameColumns { get; set; }
        public string TableTypes { get; set; }
        public bool CanCreateDB { get; set; } = false;
        public bool NormalizedNames { get; set; } = false;
        public string OverrideDataBaseId { get; set; }
    }
}
