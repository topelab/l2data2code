using L2Data2Code.SharedLib.Extensions;
using System.Globalization;

namespace L2Data2Code.BaseGenerator.Entities
{
    /// <summary>
    /// Represents an entity that could be a table, object, etc.
    /// </summary>
    /// <remarks>Some of these properties are duplicated to maintain compatibility with old templates, so it's not a good idea to remove them if you have template files with this use</remarks>
    public class Entity
    {
        /// <summary>
        /// Entity name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Use spanish lang?
        /// </summary>
        public bool UseSpanish { get; set; }
        /// <summary>
        /// Entity name in uppercase format
        /// </summary>
        public string Uppercase => Name.ToUpper(CultureInfo.CurrentCulture);
        /// <summary>
        /// Entity name in lowercase format
        /// </summary>
        public string Lowercase => Name.ToLower(CultureInfo.CurrentCulture);
        /// <summary>
        /// Entity name in plural format
        /// </summary>
        public string Plural => Name.ToPlural();
        /// <summary>
        /// Entity name in plural format
        /// </summary>
        public string ToPlural => Plural;
        /// <summary>
        /// Entity name in singular format
        /// </summary>
        public string Singular => Name.ToSingular();
        /// <summary>
        /// Entity name in singular format
        /// </summary>
        public string ToSingular => Singular;
        /// <summary>
        /// Entity name in camel case format
        /// </summary>
        public string LowerCamelCase => Name.Camelize();
        /// <summary>
        /// Entity name in camel case format
        /// </summary>
        public string Camelize => Name.Camelize();
        /// <summary>
        /// Entity name in pascal case format
        /// </summary>
        public string Pascalize => Name.Pascalize();
        /// <summary>
        /// Entity name humanize format
        /// </summary>
        public string Humanize => Name.Humanize();
        /// <summary>
        /// Entity name uncapitalized and humanized
        /// </summary>
        public string HumanizeUnCapitalize => Name.HumanizeUnCapitalize();
        /// <summary>
        /// Entity name uncapitalized
        /// </summary>
        public string UnCapitalize => Name.UnCapitalize();
        /// <summary>
        /// Entity name in plural camel case format
        /// </summary>
        public string PluralCamelize => Plural.Camelize();
        /// <summary>
        /// Entity name in singular camel case format
        /// </summary>
        public string SingularCamelize => Singular.Camelize();
        /// <summary>
        /// Has entity multiple PK columns?
        /// </summary>
        public bool MultiplePKColumns { get; set; }

        /// <summary>
        /// Entity name
        /// </summary>
        public override string ToString() { return Name; }
    }

}
