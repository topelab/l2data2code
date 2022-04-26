# L2 Data2Code 

Have a database and want to generate code?

Have an idea of data model and want to generate code for that idea?

Only needs a template, a data source (from database, from special JSON file or from an .NET assembly) and L2 Data2Code.

## What is L2 Data2Code?

A simple definition would be a code generator. But is more than a simple code generator from a data source. It gets all the info from the data source (actually, from database, from special JSON file or from an .NET assembly), then merge with a template.

A template is a set of files with a special syntax based on Mustache to define the result.
Normally, a template set is composed by a `json` file on the project root and then one or more directories where templates of files are located. This template file is referenced at `appsettings.json` file at **TemplateSettings** node.

## How its work?

In this solution we have a sample set of files and configuration template to test an initial playground. You can see a sample structure with some files at `SampleData` at current execution path of this application.

First file founded at current execution path is `appsettings.json` with a minimum structure:

- *appSettings* section
- Partial *Schemas* section

### Specification for appsettings.json

Here we will define settings for the application: data sources, templates that will use, etc... It has 5 sections: appSettings, Templates, Schemas, Areas and Modules. You can define a minimum **appsettings.json** with only *appSettings* section, and the rest of sections inside another json file (it will be specified at **TemplateSettings** key)

#### appSettings section

- **Editor**: "VSCODE" or path to an editor.
- **Editor2**: same as *Editor* to an alternative editor.
- **TemplatesBasePath**: Base path for templates.
- **UICulture**: `es-ES|ca-ES|en`: if not defined, application will try to use UI culture if is one of English, Spanish or Catalan. It can be set to `es-ES` or `ca-ES` on `en` to set UI culture for the application.
- **showVarsWindow**: `true|(false)`: when `true`, defined vars could be showed in a window.
- **generateJsonInfo**: `true|(false)`: when `true`, a JSON info will be generated from data source (if input data source is not a json file) at log file if *JsonGeneratedPath* is not defined.
- **JsonGeneratedPath**: `true|(false)`: the path on which JSON file will be generated when *generateJsonInfo* is `true`.
- **Encoding**: `(utf8)|latin1`: establish encoding for output files.
- **EndOfLine**: `(crlf)|lf|cr`: establish end of line for output files.
- **TemplateSettings**: relative path (from app running path) to file that contains other sections.

### Specification for file specified in *TemplateSettings*

#### Templates section

A collection of template settings that makes a list of templates available for the application.

![](images/2021-04-01-07-35-08.png)


Every template has a name and these properties:

- **Name**: Name for humans.
- **Path**: relative path from *TemplatesBasePath* on which resides resource templates.
- **RemoveFolders** `(true)|false`: folders will be removed before generation.
- **ResourcesFolder** `"General"`: Name of folder for resource templates (files or folders). This name is used as an identification for template. If *IsGeneral* is true, this folder will be processed globally.
- **ItemsResources** `[]`: array of folder resources that will be processed for every entity.
- **Company**: Special var that contains company name (or what else you want) to use inside template files or in *Vars* / *Configurations* / *FinalVars* specification.
- **Area**: Special var that contains area or section name (or what else you want) to use inside template files or in *Vars* / *Configurations* / *FinalVars* specification.
- **Module**: Special var that contains module name (or what else you want) to use inside template files or in *Vars* / *Configurations* / *FinalVars* specification.
- **IgnoreColumns**: Collection of columns separated by semicolons to be ignored when traversing the entity properties.
- **SavePath**: string that contains output path. It can use any defined variable inside. Example: `"c:\src\tmp\{{Company}}\{{Area}}.{{Module}}-{{Version}}"`.
- **SolutionType**: *(in future versions, SolutionType will be moved to appsettings.json)* Application can open different types of solutions, not only Visual Studio solutions, you can specify property *SolutionType* on template. This is a comma separated string with 4 fields:
    - field 1 (*AppType*): `vs` (Visual Studio), `vsc` (Visual Studio Code), `nb` (Apache Netbeans), `ec` (Eclipse), `ij` (IntelliJIdea). Default: `vs`
    - field 2 (*SearchExpression*): file pattern to search. Default `*.sln`
    - field 3 (CommandLine): program to open solution. Default `{file}`
    - field 4 (*CommandArguments*): arguments for command line used. Default empty string.
    
    We can use replacement `{file}`, `{directory}` or `{parent}` on *CommandLine* and *CommandArguments*:
    - `{file}` represents file or files found with *SearchExpression*
    - `{directory}` is the path for the file (without the file name)
    - `{parent}` is the path for parent directory.
- **NextResource**: *ResourcesFolder* name for the next template that will be resolved.
- **IsGeneral**: string with `true` or `false` indicates if this template is a general template, this collection of files will be not used for any of the entities or tables from data source.
- **PreCommands and PostCommands elements**:

  - PreCommands will be executed before output is being generated.
  - PostCommands will be executed after output is generated.
  - Both PreCommands and PostCommands have same attributes: *Name*, *Directory*, *Exec*, *ShowWindow*, *ShowMessages*, *ShowMessageWhenExitCodeNotZero* and *ShowMessageWhenExitCodeZero*. All *ShowMessage* attributes are true by default.
  - Example:
    ```json
    "PostCommands": [
        {
            "Name": "Buid",
            "Directory": "{{SavePath}}",
            "Exec": "start .",
            "ShowWindow": "false"
        }
    ],
    ```
- **Vars**: A collection of name-value values.
- **Configurations**: A collection of vars configuration that will be showed as *Template Configuration* in UI.

  ![](images/2021-04-01-07-57-02.png)

  Every var definition contains a name and a value: 

    - **Name**: *NameOfVar* indicates name of one of the configuration that will be showed as *Template Configuration* in UI.
    - **Value**: A collection of values separated by a semicolon.
- **FinalVars**: Its the same as Vars attribute, but it evaluates vars after the Vars collection.


#### Template section example

Now, we can use `Template` inside *SavePath*. In this example `Template = "Test template (global)"`

```json
    "Templates": {
        "Test Demo": {
            "Name": "Test template (global)",
            "ResourcesFolder": "GlobalFiles",
            "ItemsResources": [ "ItemFiles" ],
            "IsGeneral": "true",
            "Company": "TestCompany",
            "Area": "TestArea",
            "Module": "TestModule",
            "SavePath": "c:\\src\\tmp\\{{Template}}\\{{Area}}.{{Module}}",
            "SolutionType": "vsc,*.md,code.cmd,{directory}",
            "PostCommands": [
                {
                    "Name": "Buid",
                    "Directory": "{{SavePath}}",
                    "Exec": "start .",
                    "ShowWindow": "false"
                }
            ],
            "Vars": {
                "Version": "1.0.0",
                "VersionSuffix": "",
                "ApplicationName": "{{Area}}.{{Module}}",
                "ApplicationTitle": "{{Company}} {{Area.Humanize}} {{Module}}",
                "SetDomain": "",
                "SetAdapters": "",
                "SetBusiness": "",
                "SetServices": "",
                "SetUseCases": "",
                "SetWebApiControllers": "",
                "SetWebApi": "",
                "SetUCDelete": "",
                "SetUCGet": "",
                "SetUCGetList": "",
                "SetUCInsert": "",
                "SetUCUpdate": "",
                "SetDefaultModels": "1"
            },
            "Configurations": {
                "Services (no UseCases)": "SetServices=1;",
                "Business (no UseCases)": "SetBusiness=1;",
                "Adapters (no UseCases)": "SetAdapters=1;",
                "Web Api (only GetList)": "SetWebApi=1;SetUCGetList=1;",
                "Api Controllers (only GetList)": "SetWebApiControllers=1;SetUCGetList=1;",
                "UseCases (only GetList)": "SetUseCases=1;SetUCGetList=1;",
                "Domain (only GetList)": "SetDomain=1;SetUCGetList=1;",
                "Web Api": "SetWebApi=1;SetUCDelete=1;SetUCGet=1;SetUCGetList=1;SetUCInsert=1;SetUCUpdate=1;",
                "Api Controllers": "SetWebApiControllers=1;SetUCDelete=1;SetUCGet=1;SetUCGetList=1;SetUCInsert=1;SetUCUpdate=1;",
                "Adapters": "SetAdapters=1;SetUseCases=1;SetUCDelete=1;SetUCGet=1;SetUCGetList=1;SetUCInsert=1;SetUCUpdate=1;",
                "UseCases": "SetUseCases=1;SetUCDelete=1;SetUCGet=1;SetUCGetList=1;SetUCInsert=1;SetUCUpdate=1;",
                "Domain": "SetDomain=1;SetUCDelete=1;SetUCGet=1;SetUCGetList=1;SetUCInsert=1;SetUCUpdate=1;"
            },
            "FinalVars": {
                "if database=mysql UseDB": "MySql; .UsingMySqlDB=1;",
                "if database=sqlserver UseDB": "SqlServer; .UsingSqlServerDB=1;",
                "if database=sqlite UseDB": "Sqlite; .UsingSqliteDB=1;",
                "if database=fake UseDB": "Sqlite; .UsingSqliteDB=1;",
                "if database=json UseDB": "Sqlite; .UsingSqliteDB=1;",
                "if database=object UseDB": "Sqlite; .UsingSqliteDB=1;",
                "if SetWebApi=1 SetWebApiControllers": "1",
                "if SetWebApiControllers=1 SetUseCases": "1",
                "if SetUseCases=1 SetBusiness": "1",
                "if SetServices=1 SetBusiness": "1; .SetModels={{SetDefaultModels}};",
                "if SetBusiness=1 SetAdapters": "1",
                "if SetAdapters=1 SetDomain": "1"
            }
        }
    },
```

#### Schemes section

Schema definition (defined with a **key**):

- **Key**: string used to identify a schema.
- **ConnectionString**: string that defines the connection for data source that can be a data base, a json file with a specific configuration, a path to a dll file, or blank.
- **Provider**: `System.Data.[FakeClient | SqlClient | MySqlClient | JsonClient | ObjectClient]`: provider for data source. Every provider has its own syntax for connection string:
  - `FakeClient`: blank string.
  - `SqlClient | MySqlClient`: normal connection string used with this providers.
  - `JsonClient`: path to json file (special format)
  - `ObjectClient`: path to .net assembly (dll) and optional, a namespace to filter types separate from path with a semicolon.
- **TableNameLanguage**: `(en)|es`: table and column names language on data source. This is used on gender and number functions.
- **RemoveFirstWordOnColumnNames**: `true|(false)`: If column names start with a word and an underscore, for example `ta_code`, setting this property to `true`will remove that first word, in our case `ta_` will be removed.
- **DescriptionsFile**: path to tab separate file on each line defines description for a table or a column:
    ```
    table<tab>description for a table
    table.column<tab>description for column in table
    ```
- **RenameTables**: semicolon separated named values strings `old_table_name=new_table_name`.
- **RenameColumns**: semicolon separated named values strings `old_column_name=new_column_name` or more specific `old_table_name.old_column_name=new_column_name`.
- **CanCreateDB**: `true|(false)`: this property will expose to templates its value if input data source is different that output data source.
- **NormalizedNames**: `true|(false)`: When is true, then *ColumnNameOrName* will print *Name* and *TableNameOrEntity* will print *Entity*. When is false then *ColumnNameOrName* will print *ColumnName* and *TableNameOrEntity* will print *TableName*.

```json
    "Schemas": {
        "general": {
            "ConnectionString": "",
            "Provider": "System.Data.FakeClient",
            "TableNameLanguage": "en",
            "RemoveFirstWordOnColumnNames": false,
            "DescriptionsFile": null,
            "RenameTables": null
        },
        "northwindtest": {
            "ConnectionString": "SampleData\\northwind-dbinfo.json",
            "Provider": "System.Data.JsonClient",
            "TableNameLanguage": "en",
            "RemoveFirstWordOnColumnNames": false,
            "DescriptionsFile": null,
            "RenameTables": null,
            "RenamaColumns": null
        },
        "northwind-entities": {
            "ConnectionString": "SampleData\\Northwind.ERP.Domain.dll;Northwind.ERP.Domain.Entities",
            "Provider": "System.Data.ObjectClient",
            "TableNameLanguage": "en",
            "RemoveFirstWordOnColumnNames": false,
            "DescriptionsFile": null,
            "RenameTables": null,
            "RenamaColumns": null
        }
    },

```

#### Data source section

Data source combo will be filled with information supplied at this section.

![](images/2021-09-10-19-14-05.png)

This is the Area definition (defined with a **Key**):

- **Key**: *string* used to fill combo.
- **Area**: *string* used like a var inside template files.
- **Schema**: schema key data source.
- **OutputSchema**: schema key for output data source.
- **DescriptionsSchema**: schema key where schema objects has a description (this will be obsolete)
- **Vars**: A collection of name-value values.

```json
    "DataSources": {
        "Northwind (json file)": {
            "Area": "Northwind",
            "Schema": "northwindtest"
        },
        "Northwind (assembly entities)": {
            "Area": "Northwind",
            "Schema": "northwind-entities"
        },
        "Northwind (assembly dtos)": {
            "Area": "Northwind",
            "Schema": "northwind-dtos"
        },
        "Demo": {
            "Area": "Demo",
            "Schema": "general"
        }
    }
```


#### Modules section

Module combo will be filled with information supplied at this section.

![](images/2021-09-10-19-19-32.png)

```json
    "Modules": {
        "Northwind.Test": { "Name": "Test" },
        "Demo.Test": { "Name": "Test" },
        "Demo.Info": { "Name": "Info" },
        "Demo.Module": { "Name": "Module" }
    }
```

## Build solution

You can load sln file with Visual Studio 2019 (Community it's OK) or run `dotnet build` from cmd line at directory containing sln file.

## Definitions

**Replacement context**:

```json
{
    "Replacement": {
        "Template": "string",
        "Entity": "Entity",
        "Module": "string",
        "Area": "string",
        "Company": "string",
        "TableName": "string",
        "TableNameOrEntity": "string",
        "IgnoreColumns": ["sting", "string", "..."],
        "UnfilteredColumns": [ "Property", "Property", "..." ],
        "GenerateBase": "bool",
        "GenerateReferences": "bool",
        "IsView": "bool",
        "IsUpdatable": "bool",
        "Description": "string",
        "ConnectionString": "string",
        "DataProvider": "string",
        "Vars": "Dictionary_string_object",
        "AllColumns": [ "Property", "Property", "..." ],
        "Columns": [ "Property", "Property", "..." ],
        "PersistedColumns": [ "Property", "Property", "..." ],
        "ForeignKeyColumns": [ "Property", "Property", "..." ],
        "Collections": [ "Property", "Property", "..." ],
        "HasCollections": "bool",
        "HasForeignKeys": "bool",
        "NotPrimaryKeyColumns": [ "Property", "Property", "..." ],
        "PrimaryKeys": [ "Property", "Property", "..." ],
        "NotPrimaryKeys": [ "Property", "Property", "..." ],
        "HasNotPrimaryKeyColumns": "bool",
        "HasPrimaryKeyColumns": "bool",
        "MultiplePKColumns": "bool",
        "IsWeakEntity": "bool", 
        "Keys": ["string", "string", "..."],
        "Values": ["object", "object", "..."],
        "Count": "int",
        "IsReadOnly": "bool",
        "CanCreateDB": "bool",
    }
}
```

**Entity**:

```json
    "Entity": {
        "Name" : "string",
        "UseSpanish": "bool",
        "Uppercase": "string",
        "Lowercase": "string",
        "Plural": "string",
        "ToPlural": "string",
        "Singular": "string",
        "ToSingular": "string",
        "LowerCamelCase": "string",
        "Camelize": "string",
        "Pascalize": "string",
        "Humanize": "string",
        "HumanizeUnCapitalize": "string",
        "UnCapitalize": "string",
        "PluralCamelize": "string",
        "SingularCamelize": "string",
        "MultiplePKColumns": "bool",
    }
}
```

**Property**:

```json
    "Property": {
        "Name": "string",
        "IdOrName": "string",
        "Table": "string",
        "Nullable": "bool",
        "PrimaryKey": "bool",
        "IsFirst": "bool",
        "IsLast": "bool",
        "IsForeignKey": "bool",
        "IsCollection": "bool",
        "DefaultValue": "string",
        "Type": "string",
        "OverrideDbType": "string",
        "DbTypeOverrided": "bool",
        "NullableType": "string",
        "HasMaxLength": "bool",
        "Description": "string",
        "ColumnName": "string",
        "ColumnNameOrName": "string",
        "PkOrder": "int",
        "IsAutoIncrement": "bool",
        "IsComputed": "bool",
        "MultiplePKColumns": "bool",
        "Precision": "int",
        "Scale": "int",
        "IsNumeric": "bool",
        "IsString": "bool",
        "IsDateOrTime": "bool",
        "Join": "string",
        "FromField": "string",
        "ToField": "string",
        "DbJoin": "string",
        "DbFromField": "string",
        "DbToField": "string",
        "FirstSample": "string",
        "Sample": "string",
        "NextSample": "string",
        "IsNameDifferentToColumnName": "bool",
    }

}
```


## Change history

:[changes.md](./changes.md)

## Known issues

:[issues.md](./issues.md)
