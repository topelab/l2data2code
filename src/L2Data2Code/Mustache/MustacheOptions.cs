using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.SharedLib.Helpers;

namespace Mustache
{
    internal class MustacheOptions
    {
        internal string JsonDataFile { get; set; }
        internal string TemplatePath { get; set; }
        internal string OutputPath { get; set; }
        internal string Collection { get; set; }
        public IJsonSetting JsonSetting { get; internal set; }
        public IMustacheRenderizer Mustacher { get; internal set; }
    }
}
