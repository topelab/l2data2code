using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Helpers;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public class ModulesConfiguration : BasicConfiguration<ModuleConfiguration>
    {
        public ModulesConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, GeneratorSectionLabels.MODULES)
        {
        }
    }
}
