using L2Data2Code.SharedLib.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace L2Data2Code.SharedLib.Configuration
{
    public class SchemasConfiguration : BasicConfiguration<SchemaConfiguration>
    {
        public SchemasConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, SectionLabels.SCHEMA)
        {
        }
    }
}
