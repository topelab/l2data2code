namespace L2Data2Code.SharedLib.Configuration
{
    public interface ITemplatesConfiguration : IBasicConfiguration<TemplateConfiguration>
    {
        bool HasToRemoveFolders(string key);
        string Resource(string key);
    }
}