using L2Data2Code.SharedLib.Configuration;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public interface IAreasConfiguration : IBasicConfiguration<AreaConfiguration>
    {
        string CommentSchema(string key);
        string Schema(string key);
        string OutputSchema(string key);
    }
}