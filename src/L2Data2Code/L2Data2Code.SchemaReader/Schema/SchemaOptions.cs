using L2Data2Code.SchemaReader.Lib;

namespace L2Data2Code.SchemaReader.Schema
{
    public class SchemaOptions
    {
        public string ConnectionStringKey { get; set; }
        public string ConnectionString { get; set; }
        public StringBuilderWriter SummaryWriter { get; set; }
        public string ConnectionStringForObjectDescriptionsKey { get; set; }
        public string ConnectionStringForObjectDescriptions { get; set; }

        public SchemaOptions(string connectionStringKey, StringBuilderWriter summaryWriter, string connectionStringForObjectDescriptionsKey)
        {
            ConnectionStringKey = connectionStringKey;
            SummaryWriter = summaryWriter;
            ConnectionStringForObjectDescriptionsKey = connectionStringForObjectDescriptionsKey;
        }
    }
}
