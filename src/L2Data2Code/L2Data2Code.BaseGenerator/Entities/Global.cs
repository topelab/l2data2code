using System.Xml.Serialization;

namespace L2Data2Code.BaseGenerator.Entities
{

    public class Global
    {
        [XmlAttribute]
        public string Vars { get; set; }

        [XmlAttribute]
        public string FinalVars { get; set; }

    }
}
