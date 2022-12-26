namespace L2Data2Code.BaseGenerator.Entities
{
    public class Command
    {
        public string Name { get; set; }
        public string Directory { get; set; }
        public string Exec { get; set; }
        public bool ShowWindow { get; set; }
        public bool ShowMessages { get; set; } = true;
        public bool ShowMessageWhenExitCodeNotZero { get; set; } = true;
        public bool ShowMessageWhenExitCodeZero { get; set; } = true;
        public string DependsOn { get; set; }
    }
}
