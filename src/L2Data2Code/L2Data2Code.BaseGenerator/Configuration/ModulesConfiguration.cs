using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Helpers;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public class ModulesConfiguration : BasicConfiguration<ModuleConfiguration>
    {
        public ModulesConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, GeneratorSectionLabels.MODULES)
        {
            SetVars();
            jsonSetting.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(JsonSetting.Config))
                {
                    SetVars();
                }
            };
        }

        private void SetVars()
        {
            foreach (var key in GetKeys())
            {
                this[key].Key = key;
                if (this[key].Group == null)
                {
                    this[key].Group = key;
                }
            }
        }

    }
}
