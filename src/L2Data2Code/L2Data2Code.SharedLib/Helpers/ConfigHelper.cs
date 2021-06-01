using L2Data2Code.SharedLib.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace L2Data2Code.SharedLib.Helpers
{
    public static class ConfigHelper
    {
        private static string _settingsFile = "appsettings.json";
        private static string _additionalSettingsFile;
        private static JObject _config;
        private static JsonSetting _jsonSetting;
        private static NameValueCollection _appSettings;
        private static bool _loadEnvironment;
        private static SchemasConfiguration _schemas;

        public static JObject Config
        {
            get
            {
                CheckLoad();
                return _config;
            }
        }

        public static NameValueCollection AppSettings
        {
            get
            {
                CheckLoad();
                return _appSettings;
            }
        }

        public static SchemasConfiguration Schemas
        {
            get
            {
                CheckLoad();
                return _schemas;
            }
        }

        public static void SetLoadEnvironment(bool loadEnvironment)
        {
            _loadEnvironment = loadEnvironment;
            ForceReload();

        }

        public static void SetAdditionalSettingsFile(string settingsFile)
        {
            _additionalSettingsFile = settingsFile;
            ForceReload();
        }

        public static SchemaConfiguration GetSchema(string name = null)
        {
            name ??= "localserver";
            return Schemas[name];
        }

        public static NameValueCollection ToNameValueCollection(this JToken source)
        {
            NameValueCollection valueCollection = new NameValueCollection();
            if (source == null) return valueCollection;

            foreach (JProperty item in source)
            {
                valueCollection[item.Name] = (string)item.Value;
            }
            return valueCollection;
        }

        public static HashSet<string> ToHashSet(this JToken source)
        {
            var valueCollection = new HashSet<string>();
            if (source == null) return valueCollection;
            if (source.Type.ToString().Equals("String"))
            {
                valueCollection.Add((string)source);
                return valueCollection;
            }

            foreach (string item in source)
            {
                valueCollection.Add(item);
            }
            return valueCollection;
        }

        private static void CheckLoad()
        {
            if (_jsonSetting == null)
            {
                _jsonSetting = new JsonSetting(_settingsFile);
                _config = _jsonSetting.Config;
                _appSettings = _config[SectionLabels.APP_SETTINGS].ToNameValueCollection();
                _schemas = new SchemasConfiguration(_config[SectionLabels.SCHEMA]);
            }
        }

        private static void ForceReload()
        {
            // Force reload next time using Config
            _jsonSetting = null;
            CheckLoad();
        }

    }
}
