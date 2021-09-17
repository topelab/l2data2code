using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;

namespace L2Data2Code.SharedLib.Configuration
{
    public class TemplatesConfiguration : BasicConfiguration<TemplateConfiguration>, ITemplatesConfiguration
    {
        public TemplatesConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, SectionLabels.TEMPLATES)
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

        public bool HasToRemoveFolders(string key) => this[key]?.RemoveFolders ?? true;
        public string Resource(string key) => this[key]?.ResourcesFolder ?? "General";

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
