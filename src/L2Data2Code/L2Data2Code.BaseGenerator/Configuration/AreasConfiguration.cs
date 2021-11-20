using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Helpers;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public class AreasConfiguration : BasicConfiguration<AreaConfiguration>, IAreasConfiguration
    {
        private const string DEFAULT_KEY = "localserver";
        public AreasConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, GeneratorSectionLabels.AREAS)
        {
        }

        public string Schema(string key)
        {
            var value = this[key];
            return value?.Schema ?? DEFAULT_KEY;
        }

        public string CommentSchema(string key)
        {
            var value = this[key];
            var defaultKey = Schema(key);
            return value?.DescriptionsSchema ?? defaultKey;
        }

        public string OutputSchema(string key)
        {
            var value = this[key];
            var defaultKey = Schema(key);
            return value?.OutputSchema ?? defaultKey;
        }
    }
}
