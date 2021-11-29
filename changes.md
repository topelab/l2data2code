### 2.9.321.1119

- **New**: Mustache cli, a command line utility to execute a file name and content transformation (based on "mustache") in tree of paths for every path and file.

    We have this in `data.json`

    ```json
    {
        "Dishes":[
            {
                "Dish": "Apple",
                "Type": "Postre"
            },
            {
                "Dish": "Melon",
                "Type": "Postre"
            },
            {
                "Dish": "Black rice",
                "Type": "First course"
            }
        ]
    }
    ```

    And these files:

    - templatedir
        - file1.md
        - file2.md
        - {{Type}}
            - {{Dish}}.md


    The idea is that when you run:
    ```cmd
    Mustache data.json templatedir outputdir Dishes
    ```

    It will generate:

    - outputdir
        - file1.md
        - file2.md
        - Dish
            - Apple.md
            - Melon.md
        - First course
            - Black rice.md



### 2.8.221.908

- DataSource (inside Areas.Name) renamed to Schema

### 2.8.121.801

- **New**: Removes templates.xml and transform to json file. Now only `appsettings.json` and json specified at *TemplateSettings* will be needed to set configurations for templates.
- **New**: Template definition are now in json format.

### 2.7.221.730

- SchemaReader refactor
- Added **new** ObjectSchemaReader that can read type infos from .net dll
- **New**: Separation for settings. Added new property at `appsettings.json` called *TemplateSttings* where you can specify path for template configurations.

### 2.6.1021.422

- Big internal refactor.
- New attribute for properties of entities:
  - *HtmlEncode*: encode special chars
- Rename attribe *Empty* to *IsEmpty*
- Update nuget packages:
  - "Newtonsoft.Json" Version="13.0.1"
  - "NLog" Version="4.7.9"
  - "MahApps.Metro" Version="2.4.4"
- Removes Affixer and WordList
- Schemas / RenameColumns can be specified by table: *table_name.column_name=new_column_name*
- Refactor for GeneratorAdapter, extract git responsability to GitService
- Added *ShowMessages*, *ShowMessageWhenExitCodeNotZero* and *ShowMessageWhenExitCodeZero* to *Command* element
- Removes bat files for git actions
- Added helpers And, Or and Equal
- Refactor form CommandService

### 2.6.421.227

- Refactor for ReplacementResult
- Refactor of MessagesViewModels
- Changed colors for pinned missages
- Fix lock when scrolling message panel
- Commands output are redirect to message panel
- Added *ShowWindow* to *Command* element

### 2.6.121.220

- Refactor with UnityContainer, preparing for DI
- **New template elements**: PreCommands and PostCommands

  - PreCommands will be executed before output is being generated
  - PostCommads will be executed after output is generated
  - Both PreCommands and PostCommands have same attributes: Name, Directory and Exec.
  - Example:
    ```xml
    <PostCommands>
        <Command Name="Build" Directory="{{SavePath}}" Exec="dotnet build" />
    </PostCommands>
    ```

### 2.5.221.213

- Can add helpers to MustacheHelpers to define functions that will be rendered

### 2.5.121.212

- Stop and shows errors when rendering
- New attributes for properties of entities:
  - *IsNumeric*: Indicates if a property of the entity is numeric
  - *IsString*: Indicates if a property of the entity is string
  - *IsDateOrTime*: Indicates if a property of the entity is DateTime or TimeSpan
  - *Scale*: Number of decimals for a numeric property

### 2.4.721.205

- New options in *appSettings*:
  - `Encoding: utf8 | latin1`: indicates output codification, defaults to utf8
  - `EndOfLine: crlf | lf`: indicates ends of lines in output files, defaults to system default

### 2.4.621.203

- Fix end of line en output files

### 2.4.521.203

- *AddMonth* trims initial '0' from month number and don't put '.' at the start.
- *AddBuildNumber* adds formated string *yy.Mdd*
- Updating MahApps.Metro to 2.4.3
- Updating NLog to 4.7.7
- Updating MySql.Data to 8.0.23

### 2.4.420.1215

- Add new button on command bar top open powershell at the output folder
- Fix responsability for command bar view model
- Fix table panel for error: when partial group of table/views are showed and select "All"

### 2.4.320.1213

- Add new button on command bar to open output with Visual Studio Code if it accessible via PATH environment variable

### 2.4.220.1212

- Migrate all projects to .net 5
- Do not erase directories that start with "." when generating code

### 2.4.120.1204

- Refactor for MainWindow: put command bar and tables/views panels in controls

### 2.3.1620.1118

- Fix appsettings.json monitoring

### 2.3.1520.1111

- Updating principal project (L2Data2CodeWPF) to .Net 5
- Updating MySql.Data 8.0.22
- Updating System.Management 5.0.0
- Updating MahApps.Metro 2.3.4

### 2.3.1420.1101

- Adjust visibility for toggle switch *GenerateOnlyJson* depending on data source (set to false when json or fake data source)

### 2.3.1320.1101

- When code is generated, a *git add -A* is executed

### 2.3.1220.1030

- New setting **JsonGeneratedPath**: Indicates where the JSON file wil be generated when **generateJsonInfo** is *true*
- New option on UI to generate **only** JSON file.

### 2.3.1120.1019

- Updating NLog to 4.7.5 from 4.7.3
- Updating MahApps.Metro.IconPacks.Material to 4.6.0 from 4.4.0
- Updating MahApps.Metro.IconPacks.SimpleIcons to 4.6.0 from 4.4.0

### 2.3.1020.1012

- When defining the schema, it is possible to override the types of the columns, for example, in Sqlite, the *decimal* type in c# is cast to TEXT. With this new version, it is possible to set *decimal* to become NUMERIC.
- Added properties:
    - *OverrideDbType* (string): Overwritten type to be used in the output DB.
    - *DbTypeOverrided* (bool): Indicates whether the column type has been overridden.
- SchemaFactory provides a new provider but only for output data sources: sqlite

### 2.3.920.1010

- Fixed duplicated table names

### 2.3.820.1007

- Two differents areas can use same area name but differents data sources and output data sources.
```json
    "Areas": {
        "Area 1": {
            "Name": "AreaName",
            "DataSource": "datasource_1",
            "OutputDataSource": "output_datasource_1"
        },
        "Area 2": {
            "Name": "Areaname",
            "DataSource": "datasource_2",
            "OutputDataSource": "output_datasource_3"
        }
    }

```

### 2.3.720.926

- **appSettings.canCreateOutputDB** moved to **Schemas.connectionStringKey.CanCreateDb**
- Added properties to use in templates: *ColumnNameOrName* and *TableNameOrEntity*
- Added **Schemas.connectionStringKey.NormalizedNames**. When is true, then *ColumnNameOrName* will print *Name* and *TableNameOrEntity* will print *Entity*. When is false then *ColumnNameOrName* will print *ColumnName* and *TableNameOrEntity* will print *TableName*

### 2.3.620.925

- Added option **RenameColumns** at *Schemas.connectionString* to give option to rename some fields. Format:
```json
    "Schemas": {
        "connectionStringKey": {
            "RenameColumns": "columnName1=newColumnName1;columnName2=newColumnName2..."
        }
    }

```

### 2.3.520.925

- Add new bool property *CanCreateDB* that will be true when *DataSource* and *OutputDataSource* in Areas -> area are different and **appSettings.canCreateOutputDB** is true.

### 2.3.4.828

- Fix on first time execution, sometimes list of tables and views are empty.

### 2.3.3.828

- Specification changes on *Modules* to add possibility to include/exclude tables/views:
```json
    "Modules": {
        "module": {
            "Name": "name of module",
            "IncludeTables": "Regex for inclussion of tables or views",
            "ExcludeTables": "Regex for exlussion of tables or views"
        }
    }
```

### 2.3.2.827

- Remove L2Data2CodeUI project from solution
- Specification changes on *Areas* and *Templates*
- Areas:
```json
    "Areas": {
        "area1": {
            "Name": "nameOfArea1",
            "DataSource": "connectionStringKey",
            "DescriptionsDataSource": "connectionStringKeyToDescriptions",
            "OutputDataSource": "outputConnectionStringKey"
        }
    }
```
- Templates:
```json
    "Templates": {
        "template": {
            "Path": "relative\\path\\to\\template",
            "Resource": "nameOfResource, default to General",
            "RemoveFolders": "true or false, default to true"
        },
    }
```
- Some values on *appSettings* (all that reference connections strings) and sections *ConnectionStrings* and *Providers* has been joined all together in a new section called *Schemas*:
```json
    "Schemas": {
        "connectionStringKey": {
            "ConnectionString": "connection string to connect to DB",
            "Provider": "driver for connection string",
            "TableNameLanguage": "en | es",
            "RemoveFirstWordOnColumnNames": "false | true (default false)",
            "DescriptionsFile": "path\\to\\columns_descriptions_file",
            "RenameTables": "tableName1=newTableName1;tableName2=newTableName2..."
        }
    }

```


### 2.3.1.820

- Added new option to generate code on different connection string. Actually, code is generated using input connection string (or Sqlite when fake or json is used), but, what about if we want to generate code using another connection string? Perhaps can update applicacion to specify output connection string and that connection will be used when generating code. At *appsettings > Areas > area* could be specified input and output connection strings separated by comma.

```json
    "Areas": {
        "area": "connectionString, connectionStringToDescriptions, outputConnectionString"
    },
```


### 2.3.0

- Added new schema reader **JsonSchemaReader** that enable application to read info from a JSON file. Set *appsettings > ConnectionStrings > connectionString* to the path of JSON file, *appsettings > Providers > connectionString* to *System.Data.JsonClient* and add an *appsettings > Area > area* with that *connectionString*
- Update Nlog 4.7.3, MySql.Data 8.0.20, MahApps.Metro 2.2.0, MahApps.Metro.IconPacks.Material 4.4.0, MahApps.Metro.IconPacks.SimpleIcons 4.4.0
- Update System.Data.SqlClient 4.8.2
- Some minor problems with table order

### 2.2.19.805

- A json info for tables will be generated (at log file) when *generateJsonInfo* is set to true in appsettings.json > appSettings. These would be used for the future feature **Generate from json datasource**

### 2.2.18.802

- Button to show vars, now at toolbar
- Fix vars order interpretation
- Doesn't recompile vars when start generationg code if *GeneratorCode* recives *Vars*
- *appsettings > Vars* can contain initialization set of vars

### 2.2.17.801

- Vars window visibility depending on appSettings > *showVarsWindow*
- Set vars window modal
- Changed XAML so DataContext is design mode.

### 2.2.16.731

- Can show window with compiled vars

### 2.2.15.731

- Adjust sample values on **Sample** and **NextSample**

### 2.2.14.730

- **Bug**: StringsExtensions bugs with nulls

### 2.2.13.729

- Added new property for entity fields: **Sample** and **NextSample**

  - **Sample**: outputs *1*, *new Date(2020,1,1)*, *TimeSpan.FromSeconsd(1)* or *"SAMPLE TEXT"* depending on field type.
  - **NextSample**: outputs next one sample.

### 2.2.12.728

- **New feature**: Chained templates

### 2.2.11.727

- **Bug**: When starting application and cannot connect to DB, the application hang. There was a problem when locking object *AllMessages* and *runningProcesses*

### 2.2.10.727

#### Solved bugs

- Partials files at base path are not properly used
- Conditional creation of files not working on full names with 2 or more {when...} conditions
- Vars evaluation, now, vars can use previous vars.
- UI controls disabled on long process

#### New features

- Multilingual (English, Spanish and Catalan) auto selected from UI or manually specified at appsettings.json > appSettings > UICulture
- File watcher on template file currently selected
- New panel to show messages from actions executed inside application
- Added new vars that can be used on templates: *GeneratorApplication* and *GeneratorVersion*
- Application can open different types of solutions, not only Visual Studio solutions, you can specify property *SolutionType* on template. This is a comma separated string with 4 fields:
    - field 1 (*AppType*): `vs` (Visual Studio), `vsc` (Visual Studio Code), `nb` (Apache Netbeans), `ec` (Eclipse), `ij` (IntelliJIdea). Default: `vs`
    - field 2 (*SearchExpression*): file pattern to search. Default `*.sln`
    - field 3 (CommandLine): program to open solution. Default `{file}`
    - field 4 (*CommandArguments*): arguments for command line used. Default empty string

    We can use replacement `{file}`, `{directory}` or `{parent}` on *CommandLine* and *CommandArguments*:
    - `{file}` represents file or files finded with *SearchExpression*
    - `{directory}` is the path for the file (without the file name)
    - `{parent}` is the path for parent directory.

### 2.1.4.716

- Show spin when loading data from data source (on selecting new area)

### 2.1.3.715

- Open VS button disabled when just open a solution
- Optimize ProcessManager: CPU consumption has been dropped 5%

### 2.1.2.713

- Change theme to Light.Orange
- Change checkboxes to toggle switch
- Rename of property Library (on CodeGeneratorDto) to Model

### 2.1.1.713

- When selecting a MySql source, views aren't showing (bug in select catalog).
- Adding a **new** field for entities: `IsUpdatable`. If an entity is from a table, `IsUpdatable` is always true.

### 2.1.0.711

- New UI version, L2 Data2Code now using WPF. Same funcionalities, but UI has a modern style.

### 2.0.6.629

- Added tool to convert templates from version 1 to 2 (English names).

### 2.0.5.628

- Big changes on ProcessManager to do some task is asynchrony.
- Fix some defects on appsettings.json
- Make detection of opened solutions and editors in the background.

### 2.0.4.625

- GitCommit is not working.

### 2.0.3.621

- Detection on change settings is not working properly.

### 2.0.3.619

- Can use fake connection in templates (var database=fake)
- Application icon changed

### 2.0.2.618

- Multilingual application: English and Spanish.

### 2.0.1.618

- Refactoring to rename properties of Replacement class, they are unified to English language:

    - Atributos: Columns
    - AtributosFK: ForeignKeyColumns
    - AtributosNotPrimaryKey: NotPrimaryKeyColumns
    - AtributosPersistibles: PersistedColumns
    - AtributosTodos: AllColumns
    - CadenaConexion: ConnectionString
    - Colecciones: Collections
    - Descripcion: Description
    - **Entidad**: Entity
    - EntidadDebil: IsWeakEntity
    - EsVista: IsView
    - GenerarBase: GenerateBase
    - HayAtributosNoPK: HasNotPrimaryKeyColumns
    - HayAtributosPK: HasPrimaryKeyColumns
    - HayColecciones: HasCollections
    - HayForeignKeys: HasForeignKeys
    - Id_o_Nombre: IdOrName
    - **Libreria**: Module
    - ListaIgnorados: IgnoreColumns
    - Nombre: Name
    - NombreColumna: ColumnName
    - NombreColumnaDiferente: IsNameDifferentToColumnName
    - NombreTabla: TableName
    - Parametros: UnfilteredColumns
    - ProveedorDatos: DataProvider
    - SeGeneranReferencias: GenerateReferences
    - Tabla: Table
    - Tipo: Type
    - TipoNullable: NullableType
    - UsarCastellano: UseSpanish

    Those in bold, are applied to file names too.


### 2.0.0.616

- An silent error was generated if application cannot connect to database.

### 2.0.0.615

- A tool is added to convert Template.xml into Template.json, when in the future, will be preferred to use JSON files instead of XML to define templates. At the moment, templates definitions are using XML.
- Use MySql.Data.MySqlClient instead of MySqlConnector, since it is incompatible when reading the schema.

### 2.0.0.614

- Stop using app.config to use appsettings.json
