using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Helpers;

namespace L2Data2Code.SchemaReader.Configuration
{
    public class SchemasConfiguration : BasicConfiguration<SchemaConfiguration>
    {
        public SchemasConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, ConfigurationLabels.SCHEMA)
        {
        }
    }
}
