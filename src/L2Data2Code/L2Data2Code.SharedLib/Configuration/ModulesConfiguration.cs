using L2Data2Code.SharedLib.Helpers;

namespace L2Data2Code.SharedLib.Configuration
{
    public class ModulesConfiguration : BasicConfiguration<ModuleConfiguration>
    {
        public ModulesConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, SectionLabels.MODULES)
        {
        }
    }
}
