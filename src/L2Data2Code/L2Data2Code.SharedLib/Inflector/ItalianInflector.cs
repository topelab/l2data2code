namespace L2Data2Code.SharedLib.Inflector
{
    /// <summary>
    /// Inflector for pluralize and singularize Italian nouns. 
    /// <remarks>
    /// by Fabrizio Cornelli <mailto:it-inflector@suppose.it>.
    /// </remarks>
    /// </summary>
    public class ItalianInflector : AbstractInflector
    {
        private class RuleReplacement
        {
            public string Rule { get; set; }
            public string Replacement { get; set; }
        }

        public ItalianInflector()
        {
            #region Fixture Data

            AddPlural("$", "i");
            AddPlural("a$", "e");
            AddPlural("o$", "i");
            AddPlural("e$", "i");
            AddPlural("i$", "i");
            AddPlural("um$", "a");
            AddPlural("io$", "i");

            AddPlural("co$", "chi");
            AddPlural("ca$", "che");
            AddPlural("go$", "ghi");
            AddPlural("ga$", "ghe");

            AddPlural("cio$", "ci");
            AddPlural("gio$", "gi");
            AddPlural("ico$", "ici");
            AddPlural("igo$", "igi");

            AddPlural("cia$", "ce");
            AddPlural("gia$", "ge");

            AddPlural("icia$", "icie");
            AddPlural("à$", "à");

            AddSingular("$", "");
            AddSingular("e$", "a");
            AddSingular("i$", "o");
            AddSingular("chi$", "co");
            AddSingular("che$", "ca");
            AddSingular("ghi$", "go");
            AddSingular("ghe$", "ga");

            AddIrregular("problema", "problemi");
            AddIrregular("dilemma", "dilemmi");
            AddIrregular("medium", "media");
            AddIrregular("uomo", "uomini");

            // these are not irregular, but there's no rule
            // to make singular. It depends by the gender of the word.
            AddIrregular("inglese", "inglesi");
            AddIrregular("posizione", "posizioni");
            AddIrregular("referente", "referenti");
            AddIrregular("interruttore", "interruttori");
            AddIrregular("archivio", "archivi");
            AddIrregular("indice", "indici");
            AddIrregular("automobile", "automobili");

            AddUncountable("film");
            AddUncountable("computer");
            AddUncountable("murales");
            AddUncountable("attività");
            AddUncountable("bar");
            AddUncountable("radio");
            AddUncountable("moto");
            AddUncountable("personale");

            #endregion
        }

        protected override void AddIrregular(string singular, string plural)
        {
            base.AddIrregular(singular, plural);
            base.AddIrregular(Unaccent(singular), Unaccent(plural));
        }

        protected override void AddUncountable(string word)
        {
            base.AddUncountable(word);
            base.AddUncountable(Unaccent(word));
        }

        #region Overrides of AbstractInflector

        public override string Ordinalize(string number)
        {
            return number;
        }

        #endregion
    }
}
