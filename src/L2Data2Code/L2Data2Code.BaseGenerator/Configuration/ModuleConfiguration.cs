using L2Data2Code.SharedLib.Interfaces;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public class ModuleConfiguration : IKeyed
    {
        public ModuleConfiguration()
        {
        }

        public ModuleConfiguration(string key)
        {
            Key = key ?? throw new System.ArgumentNullException(nameof(key));
            Name = string.Join('.', key.Split('.')[1..]);
        }

        public string Key { get; set; }
        public string Name { get; set; }
        public string IncludeTables { get; set; }
        public string ExcludeTables { get; set; }
        public string Group { get; set; }
    }
}
