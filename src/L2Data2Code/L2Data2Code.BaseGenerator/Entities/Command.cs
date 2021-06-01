using System.Xml.Serialization;

namespace L2Data2Code.BaseGenerator.Entities
{
    public class Command
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Directory { get; set; }

        [XmlAttribute]
        public string Exec { get; set; }

        [XmlAttribute]
        public bool ShowWindow { get; set; }

        [XmlAttribute]
        public bool ShowMessages { get; set; } = true;

        [XmlAttribute]
        public bool ShowMessageWhenExitCodeNotZero { get; set; } = true;

        [XmlAttribute]
        public bool ShowMessageWhenExitCodeZero { get; set; } = true;
    }
}
