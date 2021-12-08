using L2Data2Code.BaseGenerator.Entities;

namespace L2Data2Code.BaseGenerator.Interfaces
{
    public interface ITemplateService
    {
        string GetPath(Template template);
        TemplateLibrary TryLoad(string templatePath, string templateResource);
    }
}