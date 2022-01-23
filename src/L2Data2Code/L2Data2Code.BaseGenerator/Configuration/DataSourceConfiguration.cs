namespace L2Data2Code.BaseGenerator.Configuration
{
    public class DataSourceConfiguration
    {
        private string _outputSchema;
        private string _descriptionSchema;

        public string Area { get; set; }
        public string Schema { get; set; }
        public string OutputSchema { get => _outputSchema ?? Schema; set => _outputSchema = value; }
        public string DescriptionsSchema { get => _descriptionSchema ?? Schema; set => _descriptionSchema = value; }

        public DataSourceConfiguration()
        {
        }

        public DataSourceConfiguration(string schema)
        {
            Schema = schema;
        }
    }
}
