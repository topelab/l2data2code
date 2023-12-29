using L2Data2Code.BaseGenerator.Configuration;
using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Exceptions;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.SharedLib.Extensions;
using System;
using System.IO;
using System.Linq;

namespace L2Data2Code.BaseGenerator.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplatesConfiguration templatesConfiguration;
        private readonly IGlobalsConfiguration globalsConfiguration;

        public TemplateService(ITemplatesConfiguration templatesConfiguration, IGlobalsConfiguration globalsConfiguration)
        {
            this.templatesConfiguration = templatesConfiguration ?? throw new ArgumentNullException(nameof(templatesConfiguration));
            this.globalsConfiguration = globalsConfiguration ?? throw new ArgumentNullException(nameof(globalsConfiguration));
        }
        public string GetPath(Template template)
        {
            return Path.Combine(template.Parent.FilePath, template.ResourcesFolder).AddPathSeparator();
        }

        public TemplateLibrary TryLoad(string templatePath, string templateResource)
        {
            try
            {
                var library = Create(templatePath, templatesConfiguration);
                if (library.HasTemplate(templateResource))
                {
                    return library;
                }
                throw new CodeGeneratorException(
                    $"Template resource {templateResource} not found in templates library",
                    CodeGeneratorExceptionType.TemplateNotFound);
            }
            catch (CodeGeneratorException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new CodeGeneratorException(
                    $"Template library {templatePath} not found",
                    CodeGeneratorExceptionType.ErrorLoadingTemplate);
            }
        }

        private TemplateLibrary Create(string templatePath, ITemplatesConfiguration templatesConfiguration)
        {
            TemplateLibrary library = new()
            {
                FilePath = templatePath,
                Global = new Global
                {
                    Vars = globalsConfiguration.Vars.ToSemiColonSeparatedString(),
                    FinalVars = string.Empty
                }
            };

            foreach (var key in templatesConfiguration.GetKeys())
            {
                var template = templatesConfiguration[key];
                library.Templates.Add(new Template
                {
                    Area = template.Area,
                    Company = template.Company,
                    IgnoreColumns = template.IgnoreColumns,
                    IsGeneral = template.IsGeneral,
                    Module = template.Module,
                    Name = template.Name,
                    NextResource = template.NextResource ?? template.ItemsResources.FirstOrDefault(),
                    Partials = template.Partials,
                    PostCommands = template.PostCommands,
                    PreCommands = template.PreCommands,
                    ResourcesFolder = template.ResourcesFolder,
                    SavePath = template.SavePath,
                    SolutionType = template.SolutionType,
                    UserVariables = template.Vars.ToSemiColonSeparatedString(),
                    FinalVariables = string.Concat(template.FinalConditions),
                    Parent = library
                });

                var lastIndexOfItemsResorces = template.ItemsResources.Count - 1;
                foreach (var item in template.ItemsResources)
                {
                    var index = template.ItemsResources.IndexOf(item);

                    library.Templates.Add(new Template
                    {
                        Area = template.Area,
                        Company = template.Company,
                        IgnoreColumns = template.IgnoreColumns,
                        IsGeneral = false,
                        Module = template.Module,
                        Name = template.Name,
                        NextResource = index + 1 < lastIndexOfItemsResorces ? template.ItemsResources[index + 1] : null,
                        Partials = template.Partials,
                        PostCommands = template.PostCommands,
                        PreCommands = template.PreCommands,
                        ResourcesFolder = item,
                        SavePath = template.SavePath,
                        SolutionType = template.SolutionType,
                        UserVariables = template.Vars.ToSemiColonSeparatedString(),
                        Parent = library
                    });
                }
            }

            library.Global.FinalVars += globalsConfiguration.FinalVars.ToSemiColonSeparatedString();

            return library;
        }
    }
}
