using L2Data2Code.SharedLib.Extensions;
using System.Globalization;

namespace L2Data2Code.BaseGenerator.Entities
{
    /// <summary>
    /// Represents an entity that could be a table, object, etc.
    /// </summary>
    public class Entity
    {
        public string Name { get; set; }
        public bool UseSpanish { get; set; }
        public string Uppercase => Name.ToUpper(CultureInfo.CurrentCulture);
        public string Lowercase => Name.ToLower(CultureInfo.CurrentCulture);
        public string Plural => Name.ToPlural();
        public string ToPlural => Plural;
        public string Singular => Name.ToSingular();
        public string ToSingular => Singular;
        public string LowerCamelCase => Name.Camelize();
        public string Camelize => Name.Camelize();
        public string Pascalize => Name.Pascalize();
        public string Humanize => Name.Humanize();
        public string HumanizeUnCapitalize => Name.HumanizeUnCapitalize();
        public string UnCapitalize => Name.UnCapitalize();
        public string PluralCamelize => Plural.Camelize();
        public string SingularCamelize => Singular.Camelize();
        public bool MultiplePKColumns { get; set; }

        public override string ToString() { return Name; }
    }

}
