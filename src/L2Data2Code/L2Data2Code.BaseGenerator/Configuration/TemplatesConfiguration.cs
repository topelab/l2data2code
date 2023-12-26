using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public class TemplatesConfiguration : BasicConfiguration<TemplateConfiguration>, ITemplatesConfiguration
    {
        public TemplatesConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, GeneratorSectionLabels.TEMPLATES)
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
                this[key].Configurations = this[key].ConfigurationsConfiguration.ToNameValueCollection();
                this[key].Vars = this[key].VarsConfiguration.ToNameValueCollection();
                this[key].FinalVars = this[key].FinalVarsConfiguration.ToNameValueCollection();
            }
        }
    }
}
