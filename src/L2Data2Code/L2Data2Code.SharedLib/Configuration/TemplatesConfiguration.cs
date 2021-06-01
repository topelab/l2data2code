using System.Collections.Specialized;

namespace L2Data2Code.SharedLib.Configuration
{
    public class TemplatesConfiguration : BasicConfiguration<TemplateConfiguration>
    {
        public TemplatesConfiguration() : base(SectionLabels.TEMPLATES)
        {
        }

        public bool HasToRemoveFolders(string key) => this[key]?.RemoveFolders ?? true;
        public string Resource(string key) => this[key]?.Resource ?? "General";
    }
}
