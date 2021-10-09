namespace L2Data2Code.SharedLib.Configuration
{
    public class AreaConfiguration
    {
        private string _outputSchema;
        private string _descriptionSchema;

        public string Name { get; set; }
        public string Schema { get; set; }
        public string OutputSchema { get => _outputSchema ?? Schema; set => _outputSchema = value; }
        public string DescriptionsSchema { get => _descriptionSchema ?? Schema; set => _descriptionSchema = value; }

        public AreaConfiguration()
        {
        }

        public AreaConfiguration(string schema)
        {
            Schema = schema;
        }
    }
}
