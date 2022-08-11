using L2Data2Code.SharedLib.Configuration;

namespace L2Data2Code.SchemaReader.Configuration
{
    public interface IDataSorcesConfiguration : IBasicConfiguration<DataSourceConfiguration>
    {
        string CommentSchema(string key);
        string Schema(string key);
        string OutputSchema(string key);
    }
}