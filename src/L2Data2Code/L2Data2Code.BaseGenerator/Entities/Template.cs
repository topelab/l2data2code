using System.Collections.Generic;
using System.Xml.Serialization;

namespace L2Data2Code.BaseGenerator.Entities
{

    public class Template
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string ResourcesFolder { get; set; }

        [XmlAttribute]
        public string Company { get; set; }

        [XmlAttribute]
        public string Area { get; set; }

        [XmlAttribute]
        public string Module { get; set; }

        [XmlAttribute]
        public string IgnoreColumns { get; set; }

        [XmlAttribute]
        public string UserVariables { get; set; }
        
        [XmlAttribute]
        public string SavePath { get; set; }

        [XmlAttribute]
        public string SolutionType { get; set; } = "vs,*.sln";

        [XmlAttribute]
        public string NextResource { get; set; }

        [XmlAttribute]
        public bool IsGeneral { get; set; }

        public List<Command> PreCommands { get; set; } = new List<Command>();

        public List<Command> PostCommands { get; set; } = new List<Command>();

        [XmlIgnore]
        public TemplateLibrary Parent { get; set; }
    }
}
