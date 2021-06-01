namespace L2Data2Code.SharedLib.Configuration
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
        public bool CanCreateDB { get; set; } = false;
        public bool NormalizedNames { get; set; } = false;
    }
}
