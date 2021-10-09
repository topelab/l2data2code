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
    public class BasicNameValueConfiguration : IBasicNameValueConfiguration
    {
        private readonly IJsonSetting jsonSetting;
        private readonly string list;

        private NameValueCollection valueCollection;
        private static readonly Regex ENVIRONMENT_VAR = new(
            @"(?<var>%[A-Za-z0-9]+%)",
            RegexOptions.Singleline | RegexOptions.Compiled
            );

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

        public void Merge(NameValueCollection nameValueCollection)
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

        private void SetConfiguration(JToken jToken)
        {
            valueCollection = jToken.ToNameValueCollection();
            ReplaceWithEnvironmentVars();
        }

    }
}
