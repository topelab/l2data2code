using L2Data2Code.SharedLib.Extensions;
using System.Linq;

namespace L2Data2Code.SharedLib.Configuration
{
    public class AreasConfiguration : BasicConfiguration<AreaConfiguration>
    {
        private const string DEFAULT_KEY = "localserver";
        public AreasConfiguration() : base(SectionLabels.AREAS)
        {
        }

        public string ConnectionStringKey(string key)
        {
            var value = this[key];
            return value?.DataSource ?? DEFAULT_KEY;
        }

        public string CommentConnectionStringKey(string key)
        {
            var value = this[key];
            var defaultKey = ConnectionStringKey(key);
            return value?.DescriptionsDataSource ?? defaultKey;
        }

        public string OutputConnectionStringKey(string key)
        {
            var value = this[key];
            var defaultKey = ConnectionStringKey(key);
            return value?.OutputDataSource ?? defaultKey;
        }
    }
}
