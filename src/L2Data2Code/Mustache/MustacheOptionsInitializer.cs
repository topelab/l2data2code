using L2Data2Code.SharedLib.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mustache
{
    internal class MustacheOptionsInitializer : IMustacheOptionsInitializer
    {
        private readonly IJsonSetting jsonSetting;

        public MustacheOptionsInitializer(IJsonSetting jsonSetting)
        {
            this.jsonSetting = jsonSetting ?? throw new ArgumentNullException(nameof(jsonSetting));
        }

        public JToken Initialize(MustacheOptions options)
        {
            jsonSetting.Initialize(options.JsonDataFile);
            var view = jsonSetting.Config;
            var entity = options.Collection;
            var entities = string.IsNullOrWhiteSpace(entity) ? view : view.SelectToken(entity);
            SetupEntities(entities);
            return entities;
        }

        private void SetupEntities(IEnumerable<JToken> entities)
        {
            var elements = entities.Count();
            var index = 1;
            foreach (var item in entities)
            {
                item["IsFirst"] = index == 1;
                item["IsLast"] = index == elements;
                index++;
                SetupProperty(item);
            }
        }

        private void SetupProperty(JToken item)
        {
            foreach (var children in item.Children())
            {
                JProperty property = children as JProperty;
                var entities = property?.Values();
                if (entities != null && entities.Count() > 1 && entities.First() is JObject)
                {
                    SetupEntities(entities);
                }
            }

        }

    }
}
