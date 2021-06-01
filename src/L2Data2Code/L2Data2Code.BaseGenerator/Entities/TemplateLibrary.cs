using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace L2Data2Code.BaseGenerator.Entities
{
    public class TemplateLibrary
    {
        public List<Template> Templates { get; set; } = new List<Template>();
        public Global Global { get; set; }

        [XmlIgnore]
        public string FilePath { get; set; }

        /// <summary>
        /// Obtains first template with name <paramref name="templateResource"/>
        /// </summary>
        /// <param name="templateResource">Name for searched template</param>
        /// <returns></returns>
        public Template GetTemplate(string templateResource) => Templates.FirstOrDefault(t => t.ResourcesFolder.Equals(templateResource, StringComparison.CurrentCultureIgnoreCase));

        /// <summary>
        /// Check if template with name <paramref name="templateResource"/> is in library
        /// </summary>
        /// <param name="templateResource">Name for searched template</param>
        /// <returns></returns>
        public bool HasTemplate(string templateResource) => Templates.Any(t => t.ResourcesFolder.Equals(templateResource, StringComparison.CurrentCultureIgnoreCase));
    }
}
