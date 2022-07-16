using Newtonsoft.Json.Linq;

namespace Mustache
{
    internal interface IMustacheOptionsInitializer
    {
        JToken Initialize(MustacheOptions options);
    }
}