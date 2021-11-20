using L2Data2Code.SharedLib.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace L2Data2Code.BaseGenerator.Configuration
{
    public interface IGlobalsConfiguration : IBasicConfiguration<JToken>
    {
        NameValueCollection Vars { get; }
        NameValueCollection FinalVars { get; }
    }
}
