using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;

namespace L2Data2Code.SchemaReader.Configuration
{
    public class SchemasConfiguration : BasicConfiguration<SchemaConfiguration>
    {
        public SchemasConfiguration(IJsonSetting jsonSetting) : base(jsonSetting, ConfigurationLabels.SCHEMA)
        {
            this["general"] = new SchemaConfiguration { ConnectionString = string.Empty, Provider = "System.Data.FakeClient", TableNameLanguage = "en", RemoveFirstWordOnColumnNames = false, };
            SetVars();
            jsonSetting.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(JsonSetting.Config))
                {
                    SetVars();
                }
            };
        }

        private void SetVars()
        {
            foreach (var key in GetKeys())
            {
                this[key].BigTables = this[key].BigTablesConfiguration.ToListOf<BigTable>();
            }
        }

    }
}
