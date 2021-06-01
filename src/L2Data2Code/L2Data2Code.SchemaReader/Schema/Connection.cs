using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Helpers;
using System;
using System.Configuration;

namespace L2Data2Code.SchemaReader.Schema

{
    /// <summary>Connection</summary>
    public class Connection : SchemaConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="connectionStringKey">The connection string key.</param>
        /// <exception cref="ConfigurationErrorsException">
        /// Not found \"{SectionLabels.SCHEMA}\" section in appsettings.json file.
        /// or
        /// Section \"{SectionLabels.SCHEMA}\" does not contain connection key \"{connectionStringKey}\" defined in appsettings.json file.
        /// </exception>
        public Connection(string connectionStringKey)
        {
            var schema = ConfigHelper.Schemas;

            if (schema.Count == 0)
            {
                throw new ConfigurationErrorsException($"Not found \"{SectionLabels.SCHEMA}\" section in appsettings.json file.");
            }

            if (schema[connectionStringKey] == null)
            {
                throw new ConfigurationErrorsException($"Section \"{SectionLabels.SCHEMA}\" does not contain connection key \"{connectionStringKey}\" defined in appsettings.json file.");
            }

            var schemaInfo = schema[connectionStringKey];
            ConnectionString = schemaInfo.ConnectionString;
            Provider = schemaInfo.Provider;
            RemoveFirstWordOnColumnNames = schemaInfo.RemoveFirstWordOnColumnNames;
            RenameTables = schemaInfo.RenameTables;
            TableNameLanguage = schemaInfo.TableNameLanguage;
            DescriptionsFile = schemaInfo.DescriptionsFile;

            if (ConnectionString.Contains("|DataDirectory|"))
            {
                //have to replace it
                string dataFilePath = GetDataDirectory();
                ConnectionString = ConnectionString.Replace("|DataDirectory|", dataFilePath);
            }


        }

        /// <summary>Gets the data directory.</summary>
        /// <returns></returns>
        public string GetDataDirectory()
        {
            return System.IO.Directory.GetCurrentDirectory() + "\\App_Data\\";
        }



    }
}
