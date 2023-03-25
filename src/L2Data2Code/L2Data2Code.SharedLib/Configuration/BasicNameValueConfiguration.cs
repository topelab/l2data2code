using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace L2Data2Code.SharedLib.Configuration
{
    public partial class BasicNameValueConfiguration : IBasicNameValueConfiguration
    {
        private readonly IJsonSetting jsonSetting;
        private readonly string list;

        private NameValueCollection valueCollection;

        public const string APP_SETTINGS_FILE = "appsettings.json";
        public const string APP_SETTINGS = "appSettings";

        public BasicNameValueConfiguration(IJsonSetting jsonSetting, string list)
        {
            this.jsonSetting = jsonSetting;
            this.list = list;
            SetConfiguration(jsonSetting.Config[list]);

            jsonSetting.PropertyChanged += JsonSetting_PropertyChanged;
        }

        private void JsonSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(JsonSetting.Config))
            {
                SetConfiguration(jsonSetting.Config[list]);
            }
        }
        public IEnumerable<string> GetKeys()
        {
            return valueCollection.AllKeys.AsEnumerable();
        }

        public string FirstOrDefault()
        {
            return valueCollection.Count > 0 ? valueCollection[0] : null;
        }

        public string this[string key] { get => valueCollection[key]; set => valueCollection[key] = value; }

        public void Merge(params string[] additionalSettingFiles)
        {
            jsonSetting.AddSettingFiles(additionalSettingFiles);
            Merge(jsonSetting.Config[APP_SETTINGS].ToNameValueCollection());
        }

        private void Merge(NameValueCollection nameValueCollection)
        {
            foreach (var key in nameValueCollection.AllKeys)
            {
                this[key] = nameValueCollection[key];
            }
            ReplaceWithEnvironmentVars();
        }

        private void ReplaceWithEnvironmentVars()
        {
            foreach (var item in GetKeys())
            {
                if (this[item].Contains('%'))
                {
                    var matches = EnvironmentVarRegex().Matches(this[item]);
                    foreach (var element in matches.Cast<Match>())
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

        private void SetConfiguration(JToken jToken)
        {
            valueCollection = jToken.ToNameValueCollection();
            ReplaceWithEnvironmentVars();
        }

        [GeneratedRegex("(?<var>%[A-Za-z0-9]+%)", RegexOptions.Compiled | RegexOptions.Singleline)]
        private static partial Regex EnvironmentVarRegex();
    }
}
