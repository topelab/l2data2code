namespace L2Data2Code.SharedLib.Configuration
{
    public interface IAreasConfiguration : IBasicConfiguration<AreaConfiguration>
    {
        string CommentSchema(string key);
        string Schema(string key);
        string OutputSchema(string key);
    }
}