using System;

namespace L2Data2Code.SharedLib.Inflector
{
	public class DataDictionaryRuleApplier: IRuleApplier
	{
		private readonly string className;
		private readonly string dataName;

		public DataDictionaryRuleApplier(string className, string dataName)
		{
            this.className = className ?? throw new ArgumentNullException(nameof(className));
			this.dataName = dataName ?? throw new ArgumentNullException(nameof(dataName));
		}

		public string Apply(string word)
		{
			return className.Equals(word) ? dataName : null;
		}
	}
}