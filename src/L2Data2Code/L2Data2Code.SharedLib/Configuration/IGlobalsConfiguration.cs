using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace L2Data2Code.SharedLib.Configuration
{
    public interface IGlobalsConfiguration : IBasicConfiguration<JToken>
    {
        NameValueCollection Vars { get; }
        NameValueCollection FinalVars { get; }
    }
}
