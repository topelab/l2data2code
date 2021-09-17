using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace L2Data2Code.SharedLib.Configuration
{
    public class GlobalsConfiguration : BasicConfiguration<JToken>, IGlobalsConfiguration
    {
        public NameValueCollection Vars { get; private set; }
        public NameValueCollection FinalVars { get; private set; }

        public GlobalsConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, SectionLabels.GLOBAL)
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
            Vars = this[nameof(Vars)].ToNameValueCollection();
            FinalVars = this[nameof(FinalVars)].ToNameValueCollection();
        }
    }
}
