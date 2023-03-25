using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace L2Data2Code.SharedLib.Inflector
{
    public partial class DefaultTableizeRuleApplier : IRuleApplier
    {
        private readonly IInflector inflector;

        public DefaultTableizeRuleApplier(IInflector inflector)
        {
            this.inflector = inflector;
        }

        public string Apply(string className)
        {
            StringBuilder builder = new(className.Length);
            var words = className.SplitWords().ToList();
            var toPluralizeIdx = words.FindLastIndex(word => !WhiteSpacesRegex().IsMatch(word));
            for (var i = 0; i < words.Count; i++)
            {
                var word = words[i];
                if (i == toPluralizeIdx)
                {
                    word = inflector.Pluralize(word);
                }
                builder.Append(inflector.Unaccent(word));
            }
            return builder.ToString();
        }

        [GeneratedRegex("[_\\s]", RegexOptions.Compiled)]
        private static partial Regex WhiteSpacesRegex();
    }
}