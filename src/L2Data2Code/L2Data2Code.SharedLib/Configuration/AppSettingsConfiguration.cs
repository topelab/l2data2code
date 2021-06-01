using System;
using System.Text.RegularExpressions;

namespace L2Data2Code.SharedLib.Configuration
{
    public class AppSettingsConfiguration : BasicNameValueConfiguration
    {
        private static readonly Regex ENVIRONMENT_VAR = new Regex(
            @"(?<var>%[A-Za-z0-9]+%)",
            RegexOptions.Singleline | RegexOptions.Compiled
            );
        public AppSettingsConfiguration() : base(SectionLabels.APP_SETTINGS)
        {
            foreach (var item in GetKeys())
            {
                if (this[item].Contains("%"))
                {
                    var matches = ENVIRONMENT_VAR.Matches(this[item]);
                    foreach (Match element in matches)
                    {
                        var environmentVar = element.Value.Trim('%');
                        var variable = Environment.GetEnvironmentVariable(environmentVar);

                        if (variable != null)
                        {
                            this[item] = this[item].Replace(element.Value, variable);
                        }
                    }

                }
            }
        }
    }
}
