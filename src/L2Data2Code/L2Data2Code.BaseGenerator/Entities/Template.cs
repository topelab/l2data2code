using System.Collections.Generic;

namespace L2Data2Code.BaseGenerator.Entities
{

    public class Template
    {
        public string Name { get; set; }
        public string ResourcesFolder { get; set; }
        public string Company { get; set; }
        public string Area { get; set; }
        public string Module { get; set; }
        public string IgnoreColumns { get; set; }
        public string UserVariables { get; set; }
        public string FinalVariables { get; set; }
        public string SavePath { get; set; }
        public string SolutionType { get; set; } = "vs,*.sln";
        public string NextResource { get; set; }
        public bool IsGeneral { get; set; }
        public List<Command> PreCommands { get; set; } = new List<Command>();
        public List<Command> PostCommands { get; set; } = new List<Command>();
        public TemplateLibrary Parent { get; set; }
    }
}
