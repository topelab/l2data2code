using Newtonsoft.Json.Linq;

namespace L2Data2Code.CLIBase.Options
{
    public interface ICLIOptionsInitializer
    {
        JToken Initialize(ICLIOptions options);
    }
}