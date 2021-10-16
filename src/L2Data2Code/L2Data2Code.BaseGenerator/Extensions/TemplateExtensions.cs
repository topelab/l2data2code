using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Exceptions;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using System;
using System.IO;
using System.Linq;
using Unity;

namespace L2Data2Code.BaseGenerator.Extensions
{
    public static class TemplateExtensions
    {
        public static string GetPath(this Template template)
        {
            return Path.Combine(template.Parent.FilePath, template.ResourcesFolder).AddPathSeparator();
        }

        public static TemplateLibrary TryLoad(this string templatePath, string templateResource)
        {
            var templatesConfiguration = ContainerManager.Container.Resolve<ITemplatesConfiguration>();

            try
            {
                var library = Initialize(templatePath, templatesConfiguration);
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

        private static TemplateLibrary Initialize(string templatePath, ITemplatesConfiguration templatesConfiguration)
        {
            var globalsConfiguration = ContainerManager.Container.Resolve<IGlobalsConfiguration>();

            TemplateLibrary library = new();

            library.FilePath = templatePath;
            library.Global = new Global
            {
                Vars = globalsConfiguration.Vars.ToSemiColonSeparatedString(),
                FinalVars = string.Empty
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
                    PostCommands = template.PostCommands,
                    PreCommands = template.PreCommands,
                    ResourcesFolder = template.ResourcesFolder,
                    SavePath = template.SavePath,
                    SolutionType = template.SolutionType,
                    UserVariables = template.Vars.ToSemiColonSeparatedString(),
                    FinalVariables = template.FinalVars.ToSemiColonSeparatedString(),
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
