using L2Data2Code.SharedLib.Configuration;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public interface ITemplatesConfiguration : IBasicConfiguration<TemplateConfiguration>
    {
        bool HasToRemoveFolders(string key);
        string Resource(string key);
    }
}