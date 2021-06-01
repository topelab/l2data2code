using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Exceptions;
using L2Data2Code.SharedLib.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace L2Data2Code.BaseGenerator.Extensions
{
    public static class TemplateExtensions
    {
        public static string GetPath(this Template template)
        {
            return Path.Combine(Path.GetDirectoryName(template.Parent.FilePath), template.ResourcesFolder).AddPathSeparator();
        }

        public static void SaveAs(this TemplateLibrary library, string fileName)
        {
            var serializer = new XmlSerializer(library.GetType());
            using (var writer = XmlWriter.Create(fileName))
            {
                serializer.Serialize(writer, library);
            }
            library.FilePath = fileName;
        }

        public static void Save(this TemplateLibrary library)
        {
            library.SaveAs(library.FilePath);
        }

        public static TemplateLibrary Load(string fileName)
        {
            var serializer = new XmlSerializer(typeof(TemplateLibrary));
            using (var reader = XmlReader.Create(fileName))
            {
                var library = (TemplateLibrary)serializer.Deserialize(reader);
                foreach (var item in library.Templates)
                {
                    item.Parent = library;
                }

                library.FilePath = fileName;

                return library;
            }
        }

        public static TemplateLibrary TryLoad(string templatePath, string templateResource)
        {
            TemplateLibrary library = null;
            try
            {
                library = Load(Path.Combine(templatePath, "Templates.xml"));
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
                    $"Template library {Path.Combine(templatePath, "Templates.xml")} not found",
                    CodeGeneratorExceptionType.ErrorLoadingTemplate);
            }
        }

    }
}
