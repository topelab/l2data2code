## 3.5.5 - L2Data2Code.Avalonia

- If code has git remote, try to pull before run commands

## 3.5.4 - L2Data2Code.Avalonia

- Fix show errors on Command execution

## 3.5.3

- Updating System.Text.Json to 9.0.7
- Updating Topelab.Core.Resolver to 1.8.9
- Updating Topelab.Core.Resolver.Microsoft to 1.8.9
- Updating System.Management to 9.0.7
- `avalonia-changes.md` moved to `changes.md` at solution root

## 3.5.2

- Upgrade System.Text.Json to 9.0.6
- Upgrade Topelab.Core.Resolver to 1.8.8
- Upgrade Topelab.Core.Resolver.Microsoft to 1.8.8
- Upgrade Handlebars.Net.Helpers to 2.5.2
- Upgrade Handlebars.Net.Helpers.Humanizer to 2.5.2
- Upgrade Handlebars.Net.Helpers.Json to 2.5.2
- Upgrade NLog to 6.0.1
- Upgrade System.Management to 9.0.6
- Upgrade Avalonia to 11.3.2
- Upgrade Avalonia.Controls.DataGrid to 11.3.2
- Upgrade Avalonia.Desktop to 11.3.2
- Upgrade Avalonia.Themes.Simple to 11.3.2
- Upgrade Avalonia.Diagnostics to 11.3.2
- Upgrade Avalonia.ReactiveUI to 11.3.2
- Upgrade Avalonia.Themes.Fluent to 11.3.2

## 3.5.1 - L2Data2Code.Avalonia

- Fix Scheme2Json with default values for arguments
- Added `HasRelatedColumns` to Replacement class

## 3.4.19 - L2Data2Code.Avalonia

- Adjusted index column handling in `SQLiteSchemaReader.cs` to fix an off-by-one error.
- Upgrade System.Text.Json to 9.0.5
- Upgrade Topelab.Core.Resolver to 1.8.7
- Upgrade Topelab.Core.Resolver.Microsoft to 1.8.7
- Upgrade System.Management to 9.0.5

## 3.4.18 - L2Data2Code.Avalonia

- Added Run button (RunApplication variable must be declared in template settings, at DataSources variables level) to run the application after generation.
- Added support for AutoCombo, and auto-complete combo box in AvaloniaUI.

## 3.4.15 - L2Data2Code.Avalonia

- Custom helpers must be prefixed with `My-` in the template.

## 3.4.14

- Upgrade System.Text.Json to 9.0.4
- Upgrade Topelab.Core.Resolver to 1.8.6
- Upgrade Topelab.Core.Resolver.Microsoft to 1.8.6
- Upgrade Handlebars.Net.Helpers to 2.5.0
- Upgrade Handlebars.Net.Helpers.Humanizer to 2.5.0
- Upgrade Handlebars.Net.Helpers.Json to 2.5.0
- Upgrade MySql.Data to 9.3.0
- Upgrade System.Management to 9.0.4
- Upgrade Avalonia to 11.3.0
- Upgrade Material.Icons.Avalonia to 2.4.1
- Upgrade Avalonia.Controls.DataGrid to 11.3.0
- Upgrade Avalonia.Desktop to 11.3.0
- Upgrade Avalonia.Themes.Simple to 11.3.0
- Upgrade Avalonia.Diagnostics to 11.3.0
- Upgrade Avalonia.ReactiveUI to 11.3.0
- Upgrade Avalonia.Themes.Fluent to 11.3.0
- Upgrade Microsoft.Data.SqlClient to 6.0.2

## 3.4.13 - L2Data2Code.Avalonia

- Added `ManualRelatedColumns` and `HasManualRelatedColumns` to the `Replacement` class.
- Introduced `ToFieldType` in `ForeignKeyColumn` for field type retrieval.
- Modified `ReplacementCollectionFactory` to improve handling of related columns.
- Cleared and repopulated `weakEntities` in `NameResolver` for accurate tracking.

## 3.4.12 - L2Data2Code.Avalonia

- Added `ToFieldType` on `ForeignKeyColumn` to get the field type of the column

## 3.4.11 - L2Data2Code.Avalonia

- Added diagnostic rules in .editorconfig for CA1822.
- Introduced `FilterPrimitive` property in `EntityColumn.cs`.
- Changed access modifiers from `private` to `internal` in `EntityTable.cs`.
- Created `EntityTablesFactory` class to manage `EntityTable` creation.
- Refactored `CodeGeneratorService` to use `IEntityTablesFactory`.
- Added new filtering properties in the `Property` class.
- Streamlined constructor and error handling in `SchemaReader`.
- Overall improvements to code structure, maintainability, and functionality.


## 3.4.10

Enhance entity filtering and add FieldIdentity property

- Added `FieldIdentity` property to the `Entity` class.
- Introduced `FilterType` and `FilterSpecification` properties in `EntityColumn` and `Property` classes.
- Updated `EntityTable` to initialize and set `FieldIdentity` based on primary key order.
- Implemented `TrySetFilterType` method for determining filter types.
- Modified `ReplacementCollectionFactory` to include new properties.
- Added `DescriptionTables` property to `SchemaConfiguration`.
- Changed `INameResolver` to return structured filtering information.
- Implemented `TrySetFilterColumn` method in `SchemaReader`.
- Created `ColumnFilter` class for encapsulating filter details.
- Updated schema reader classes to utilize new filtering logic.
- Refactored `NameResolver` to manage new properties effectively.


## 3.4.9

- When field has a relation, the property `HasRelation` is set to `true` and the property `Join` is set to table witch is related

## 3.4.8

- New `IsBig` and `IsSmall` to Replacement, where IsBig = *IsBigTables and not IsWeakEntity* and IsSmall = *not IsBigTables and not IsWeakEntity*

## 3.4.7

- Add `IsBigTable` and `FilterByColumns` to Replacement. This can be used to filter columns on big tables. On Schemes, must define a `BigTables` section, like this:

```json
{
  "Schemes": {
    "CalendarPostgreSQL": {
      "ConnectionString": "Host=localhost:5432;Database=DATABASE;User ID=USER;Password=PASS",
      "Provider": "Npgsql",
      "TableNameLanguage": "es",
      "RemoveFirstWordOnColumnNames": false,
      "DescriptionsFile": "dataSource\\calendarios-descriptions.txt",
      "WriteDescriptionsFile": true,
      "EnumTables": "TipoFestivo=Id,TipoFestivo;TipoJornada=Id,Nombre",
      "WeakEntities": "CalendarioDetalle;TipoJornadaCalendario;TipoJornadaSemana",
      "BigTables": {
        "Municipio" : {
          "ColumnsFilter": [ "IdCA", "IdProvincia" ]
        },
        "FiestaLocal" : {
          "ColumnsFilter": [ "IdMunicipio", "Anyo", "Municipio" ]
        },
        "FiestaNacional" : {
          "ColumnsFilter": [ "Fecha" ]
        },
        "FiestaAutonomica" : {
          "ColumnsFilter": [ "IdCa", "Fecha" ]
        }
      }
    }
  }
}
```

## 3.4.6

- Add `IsWeakEntity` to Replacement

## 3.4.4

- Fix `EnumTables` to get distinct name values ordered by id
- Upgrade System.Text.Json to 9.0.1
- Upgrade Topelab.Core.Resolver to 1.8.3
- Upgrade Topelab.Core.Resolver.Microsoft to 1.8.3
- Upgrade Handlebars.Net.Helpers to 2.4.10
- Upgrade Handlebars.Net.Helpers.Humanizer to 2.4.10
- Upgrade Handlebars.Net.Helpers.Json to 2.4.10
- Upgrade Microsoft.Data.SqlClient to 6.0.1
- Upgrade Material.Icons.Avalonia to 2.2.0
- Upgrade MySql.Data to 9.2.0
- Upgrade System.Management to 9.0.1

## 3.4.3

- Upgrade Topelab.Core.Resolver to 1.8.2
- Upgrade Topelab.Core.Resolver.Microsoft to 1.8.2
- Upgrade Avalonia to 11.2.3
- Upgrade Avalonia.Controls.DataGrid to 11.2.3
- Upgrade Avalonia.Desktop to 11.2.3
- Upgrade Avalonia.Themes.Simple to 11.2.3
- Upgrade Avalonia.Diagnostics to 11.2.3
- Upgrade Avalonia.ReactiveUI to 11.2.3
- Upgrade Avalonia.Themes.Fluent to 11.2.3

## 3.4.2

- Upgrade Npgsql to 9.0.2

## 3.4.1

- Upgrade Topelab.Core.Resolver to 1.8.1
- Upgrade Topelab.Core.Resolver.Microsoft to 1.8.1
- Upgrade Handlebars.Net.Helpers to 2.4.7
- Upgrade Handlebars.Net.Helpers.Humanizer to 2.4.7
- Upgrade Handlebars.Net.Helpers.Json to 2.4.7
- Upgrade Npgsql to 9.0.1
- Upgrade Avalonia to 11.2.2
- Upgrade Avalonia.Controls.DataGrid to 11.2.2
- Upgrade Avalonia.Desktop to 11.2.2
- Upgrade Avalonia.Themes.Simple to 11.2.2
- Upgrade Avalonia.Diagnostics to 11.2.2
- Upgrade Avalonia.ReactiveUI to 11.2.2
- Upgrade Avalonia.Themes.Fluent to 11.2.2

## 3.4.0

- Upgrade to .NET 9.0
- Upgrade to Topelab.Core.Resolver to 1.8.0
- Upgrade to Topelab.Core.Resolver.Microsoft to 1.8.0
- Upgrade to System.Management to 9.0.0
- Upgrade to System.Data.SqlClient to 4.9.0
- Upgrade to Avalonia to 11.2.1
- Upgrade to Avalonia.Controls.DataGrid to 11.2.1
- Upgrade to Avalonia.Desktop to 11.2.1
- Upgrade to Avalonia.Themes.Simple to 11.2.1
- Upgrade to Avalonia.Diagnostics to 11.2.1
- Upgrade to Avalonia.ReactiveUI to 11.2.1
- Upgrade to Avalonia.Themes.Fluent to 11.2.1

## 3.3.27

- Fix `IsAutoIncrement` on table columns from a PostgreSQL database

## 3.3.26

- Upgrade Topelab.Core.Resolver to 1.7.1
- Upgrade Topelab.Core.Resolver.Microsoft to 1.7.1
- Upgrade Handlebars.Net.Helpers to 2.4.6
- Upgrade Handlebars.Net.Helpers.Humanizer to 2.4.6
- Upgrade Handlebars.Net.Helpers.Json to 2.4.6
- Upgrade System.Data.SQLite.Core to 1.0.119
- Upgrade Avalonia to 11.1.4
- Upgrade MessageBox.Avalonia to 3.1.6.13
- Upgrade Avalonia.Controls.DataGrid to 11.1.4
- Upgrade Avalonia.Desktop to 11.1.4
- Upgrade Avalonia.Themes.Simple to 11.1.4
- Upgrade Avalonia.Diagnostics to 11.1.4
- Upgrade Avalonia.ReactiveUI to 11.1.4
- Upgrade Avalonia.Themes.Fluent to 11.1.4

## 3.3.25

- Updating topelab.core.resolver.microsoft to 1.7.0
- Updating topelab.core.resolver to 1.7.0
- Updating NLog to 5.3.4

## 3.3.22

- Updating Avalonia to 11.1.3
- Updating MessageBox to 3.1.6

## 3.3.21

- Updating topelab.core.resolver.microsoft to 1.6.1
- Updating Handlebars.Net.Helpers to 2.4.5
- Updating Handlebars.Net.Helpers.Humanizer to 2.4.5
- Updating Handlebars.Net.Helpers.Json to 2.4.5
- Updating MySql.Data to 9.0.0
- Updating Avalonia to 11.1.0
- Updating Avalonia.Controls.DataGrid to 11.1.0
- Updating Avalonia.Desktop to 11.1.0
- Updating Avalonia.Themes.Simple to 11.1.0
- Updating Avalonia.Diagnostics to 11.1.0
- Updating Avalonia.ReactiveUI to 11.1.0
- Updating Avalonia.Themes.Fluent to 11.1.0
- Updating topelab.core.resolver to 1.6.1

## 3.3.20

- Added properties `IdentifiableById`, `HasOnlyOnePKColumn` and `IsIdentifiable` to `Entity`

## 3.3.19

- Renew helper `IncreaseVersion increment VersionVar` where `increment` must be between -100 and 100, and `VersionVar` is a var name with version number. By default, `increment` is 1 and `VersionVar` is the value of var `Version`

## 3.3.18

- Added `Setting.RemoveFolderExceptions`, an list of directory names that will not be removed even if `RemoveFolders` is `true`
- Added `FinalConditions` to `Global`

## 3.3.16

- Added management for extensions `.xaml` and `.axaml` as `.xml`

## 3.3.15

- Now is possible to use `foreach(Collection)filename{{Property}}.ext` as a file name at entity level and L2Data2Code will create a collection of files using `Collection[].Property` where `Collection` is any collection in `Entity` (like `NotRelatedColumns`) and `Property` is any property of item inside collection.

## 3.3.14

- Added `DistinctForeignKeyColumnsByType`

## 3.3.13

- Binary files can be used in templates with a '!' as a first char of name. Content will be copied without transform

## 3.3.12

- Updating all Avalaonia packages to 11.0.11
- Updating all HandleBars packages to 2.4.4
- Move PostCommands execution after internal commit
- New helper `IncreaseVersion [increment = 1]`

## 3.3.11

- Better use for descriptions files: added `bool WriteDescriptionsFile` to schema configuration. When is `true` a description file for all columns is written on `(template_dir)\dataSource\(schemaName)-descriptions.txt`. **Now** it's easy to fill descriptions for columns.

## 3.3.10

- Added management for extensions `.resx` as `.xml`

## 3.3.9

- Remove extra lines equals to previous
- Added property `ShortName` for relations

## 3.3.8

- Added property `HasRelation` to `Entity` and `NotRelatedColumns` for replacement
- Added `FieldDescriptor` and `FirstPK` to `Entity`

## 3.3.7

- Update some packages

## 3.3.6

- Fixed some bugs on PostgreSQL schema reader
- Updating Handlebars

## 3.3.5

- Update topelab.core.resolver.microsoft to 1.5.5
- Update Prism.Core to 9.0.401-pre
- Update Dapper to 2.1.35
- Update Avalonia to 11.0.10
- Update Avalonia.Controls.DataGrid to 11.0.10
- Update Avalonia.Desktop to 11.0.10
- Update Avalonia.Themes.Simple to 11.0.10
- Update Avalonia.Diagnostics to 11.0.10
- Update Avalonia.ReactiveUI to 11.0.10
- Update Avalonia.Themes.Fluent to 11.0.10
- Update topelab.core.resolver to 1.5.5


## 3.3.3

- Updating Avalonia to 11.0.9
- Updating Avalonia.Controls.DataGrid to 11.0.9
- Updating Avalonia.Desktop to 11.0.9
- Updating Avalonia.Themes.Simple to 11.0.9
- Updating Avalonia.Diagnostics to 11.0.9
- Updating Avalonia.ReactiveUI to 11.0.9
- Updating Avalonia.Themes.Fluent to 11.0.9


## 3.3.2

- Updating topelab.core.resolver.microsoft to 1.5.4
- Updating topelab.core.resolver to 1.5.4
- Fix refresh on data source selected

## 3.3.1

- Updating NLog to 5.2.8
- Updating Handlebars.Net.Helpers to 2.4.1
- Updating MySql.Data to 8.3.0
- Updating System.Data.SqlClient to 4.8.6
- Updating Avalonia to 11.0.7

## 3.3.0

### Summary of changes on templates from 3.2.3

1. `PostCommands` and `PreCommands` are now key elements in place of an array, and previous *name* is now key of the element:

    *Before*

    ```json
          "PostCommands": [
            {
              "Name": "Build",
              "Directory": "{{SavePath}}",
              "Exec": "dotnet build -v quiet",
              "ShowWindow": false
            },
            {
              "Name": "CopyNuPkg",
              "DependsOn": "Build",
              "Directory": "{{SavePath}}",
              "Exec": "to-local-repo.ps1",
              "ShowWindow": false,
              "Skip": "{{CopyNuPkgSkip}}"
            }
          ]
    ```

    *Now*

    ```json
          "PostCommands": {
            "Build": {
              "Directory": "{{SavePath}}",
              "Exec": "dotnet build -v quiet",
              "ShowWindow": false
            },
            "CopyNuPkg": {
              "DependsOn": "Build",
              "Directory": "{{SavePath}}",
              "Exec": "to-local-repo.ps1",
              "ShowWindow": false,
              "Skip": "{{CopyNuPkgSkip}}"
            }
          }
    ```

1. `Configurations` has been transformed to `Settings` with a key than can be used inside *DataSources* without repeating vars expressions.

    *Before*

    ```json
      "Configurations": {
        "Services (no UseCases, No models)": "SetServices=true;SetDefaultModels=false;",
        "Services (no UseCases)": "SetServices=true;"
      }
    ```

    *Now*

    ```json
      "Settings": {
        "ServicesNoUseCasesNoModels": {
          "Name": "Services (no UseCases, No models)",
          "Vars": {
            "SetServices": true,
            "SetDefaultModels": false
          }
        },
        "ServicesNoUseCases": {
          "Name": "Services (no UseCases)",
          "Vars": {
            "SetServices": true
          }
        }
      }
    ```

    Setting key (like *ServicesNoUseCasesNoModels*) will be usable in *DataSources*

1. `FinalVars` are now `FinalConditions` with a more readable syntax:

    *Before*

    ```json
      "FinalVars": {
        "if SetWebApi=true SetWebApiControllers": true,
        "if SetWebApiControllers=true SetUseCases": true,
        "if SetUseCases=true SetBusiness": true
      }
    ```

    *Now*

    ```json
      "FinalConditions": {
        "010": {
          "When": "SetWebApi",
          "Eq": true,
          "Then": {
            "SetWebApiControllers": true
          }
        },
        "020": {
          "When": "SetWebApiControllers",
          "Eq": true,
          "Then": {
            "SetUseCases": true
          }
        },
        "030":{
          "When": "SetUseCases",
          "Eq": true,
          "Then": {
            "SetBusiness": true
          }
        }
      }
    ```

1. `DataSources` has a new element `Modules`, that replace root child `Modules`. This will permit define modules for different data sources. `Settings` is a new element where we can attach a module to a specified `Setting`:

    *Before*

    ```json
      "DataSources": {
        "Calendarios (json-sqlite)": {
          "Area": "Topelab",
          "DefaultModule": "Topelab.CalendarLite",
          "Schema": "calendariosjson",
          "OutputSchema": "calendariossqlite",
          "Vars": {
            "CopyNuPkgSkip" : false
          }
        },
      }
    ```

    *Now*

    ```json
      "DataSources": {
        "Calendarios (json-sqlite)": {
          "Area": "Topelab",
          "DefaultModule": "Topelab.CalendarLite",
          "Modules": {
            "Topelab.CalendarLite": {
              "Name": "CalendarLite"
            },
            "Topelab.CalendarLiteAPI": {
              "Name": "CalendarLiteAPI"
            }
          },
          "Schema": "calendariosjson",
          "OutputSchema": "calendariossqlite",
          "Vars": {
            "CopyNuPkgSkip": false,
            "RemoveFolders": true
          },
          "Settings": {
            "ServicesNoUseCasesNoModels": "Topelab.CalendarLite",
            "WebAPI": "Topelab.CalendarLiteAPI"
          }
        },
      }
    ```


## 3.2.4

- Added possibility to group name spaces using module groups
- **New**: Added `ModulesGroup` to `DataSources`
- **New**: Added `Group` to `Modules`

## 3.2.3

- Updating Material.Icons.Avalonia to 2.1.0
- Removed WPF and MAUI projects, focalize on AvaloniaUI only

## 3.2.2

- Updating Handlebars.Net.Helpers to 2.4.1.2
- Updating Handlebars.Net.Helpers.Humanizer to 2.4.1.2
- Updating Handlebars.Net.Helpers.Json to 2.4.1.2

## 3.2.1

- Updating topelab.core.resolver.microsoft to 1.5.3
- Updating NLog to 5.2.7
- Updating Avalonia to 11.0.6
- Updating Avalonia.Controls.DataGrid to 11.0.6
- Updating Avalonia.Desktop to 11.0.6
- Updating Avalonia.Themes.Simple to 11.0.6
- Updating Avalonia.Diagnostics to 11.0.6
- Updating Avalonia.ReactiveUI to 11.0.6
- Updating Avalonia.Themes.Fluent to 11.0.6
- Updating topelab.core.resolver to 1.5.3


## 3.2.0

- Updating to .NET 8

## 3.1.6

- **New**: Added property `Command.Skip` useful to skip commands on `Templates.PreCommands` or `Templates.PostCommands`. Can use variables `Vars` defined at `DataSources` level.

## 3.1.5

- Fix remove folders: when is set to false on template it doesn't allow activate on run
- Fix relation names on tables

## 3.1.3

- **New**: Added `EnumTables` on `Schemes[]` used to mark tables to read its values (`Id` and `Name`) into a `List<EnumTableValue>`
- **New**: Added new alises for strings `ToPascalCleanName` and `ToCamelCleanName`

## 3.1.2

- Fix SQLite reader

## 3.0.5

- Updating topelab.core.resolver.microsoft to 1.4.13
- Updating Material.Icons.Avalonia to 2.0.1
- Updating NLog to 5.2.0
- Updating System.Data.SQLite.Core to 1.0.118
- Updating System.Management to 7.0.2
- Updating topelab.core.resolver to 1

## 3.0.4

- **New**: Added capability to get indexes information from DB

## 3.0.3

- Updating Topelab.Core.Resolver to 1.4.12
- Updating Topelab.Core.Resolver.Microsoft to 1.4.12
- Updating NLog to 5.1.4
- Updating Avalonia to 0.10.21
- Updating Avalonia.Controls.DataGrid to 0.10.21
- Updating Avalonia.Desktop to 0.10.21
- Updating Avalonia.Diagnostics to 0.10.21
- Updating Avalonia.ReactiveUI to 0.10.21


## 3.0.2

- Fixed problems when parent directory for an output path that doesn't exist

## 3.0.1

- Fix some issues with buttons in command bar

## 3.0.0

- Renamed main application L2 Data2Code WPF to L2 Data2Code
- All views migrated to AvaloniaUI

## 2.13.12

- Updating Topelab.Core.Resolver to 1.4.10
- Updating Topelab.Core.Resolver.Microsoft to 1.4.10
- Updating Handlebars.Net to 2.1.3
- Updating NLog to 5.1.2

## 2.13.11

- **New**: New vars added: `MinorVersion` (2.13) and `MajorVersion` (2)

## 2.13.10

- Updating NLog to 5.1.1
- Updating MySql.Data to 8.0.32

## 2.13.9

- Fix error processing two or more templates with same resource folder

## 2.13.8

- Updating Topelab.Core.Resolver to 1.4.8
- Added dependencies on commands

## 2.13.7

- Updating Topelab.Core.Resolver to 1.4.7
- Updating Topelab.Core.Resolver.Microsoft to 1.4.7
- Updating Handlebars.Net.Helpers to 2.3.12
- Updating Handlebars.Net.Helpers.Humanizer to 2.3.12
- Updating Newtonsoft.Json to 13.0.2
- Updating Handlebars.Net.Helpers.Json to 2.3.12
- Updating NLog to 5.1.0
- Updating System.Data.SQLite.Core to 1.0.117


## 2.13.6

- Updating Topelab.Core.Resolver to 1.4.5

## 2.13.5

- **New**: Added `OverrideDataBaseId` on `Schemes[]` witch is used to override `database` at `Vars` and it can be used when generating code.

## 2.13.4

- Updating to .NET 7.0

## 2.13.3

- Some fixes in schema readers (mysql & sqlserver)

## 2.13.2

- Now, *SavePath* can use any of the vars defined

## 2.13.1

- Added personalized configurations to data sources
- Updated MySql.Data to 8.0.31

## 2.13.0

- Added personalized table types to schemes that can be used elsewhere in templates.
- Updated some NuGet packages

## 2.12.6

- Added default values to columns (experimental)

## 2.12.5

- Internal refactors to decouples scheme database reading service.
- **Breaking change**: "Schemas" renamed to "Schemes", **is necessary to be changed on template settings files**.

## 2.12.4

- Added `DefaultModule` to `DataSources`. This must match a Modules key.

## 2.12.3

- Fixed conditionals on `template-settings.json` and in pattern files that start with `where{{...}}`

## 2.12.2

- **New**: Added options in L2DataToCode to set partials path.
- Putting helpers into CustomHelpers.
- Removed references to Stubble.

## 2.11.1

- Generated JSON redesign
- Added to MustacheHelpers *Join* with three variants:
  - {{Join separator}}: Takes an array a join with a separator.
  - {{JoinWithHeader separator header}}: Takes an array a join with a separator starting with a header.
  - {{JoinWithHeaderFooter separator header footer}}: Takes an array a join with a separator starting with a header and ending with a footer.
  - *separator, header and footer* are strings
- Mustache add *IsFirst* and *IsLast* to nodes at JObject[] objects
- Updating NLog to 5.0.1 from 5.0.0

## 2.11.0

- Updating to Topelab.Core 1.4.0
- Changed version numbering

## 2.10.522.531

- Connection strings for JsonClient and ObjectClient providers are now relative to template path
- **New**: Added new var *TemplatePath*

## 2.10.422.504

- Update NuGet packages.

## 2.10.322.423

- **New**: Added extension *DoubleSlash* to double back slash inside quote string


## 2.10.222.225

- Fix functionality for GenerateOnlyJson
- Updating NLog

## 2.10.122.123

- **Breaking change**: "Areas" section needs to be renamed to "DataSources", and any "Name" element inside, needs to be renamed to "Area".

## 2.9.522.121

- Update NuGet packages.
- Adapted to version 1.3.1122.121 of Topelab.Core.Resolver

## 2.9.422.116

- **New**: Refactors to use Topelab.Core.Resolver (not in NUGET public repository, need to be copied local)

## 2.9.321.1208

- **New**: Refactors and upgrade to .Net 6
- **New**: *Templates.Template.SavePath* can use name of template using var *Template*

## 2.8.221.908

- DataSource (inside Areas.Name) renamed to Schema

## 2.8.121.801

- **New**: Removes templates.xml and transform to JSON file. Now only `appsettings.json` and JSON specified at *TemplateSettings* will be needed to set configurations for templates.
- **New**: Template definition are now in JSON format.

## 2.7.221.730

- SchemaReader refactors
- Added **new** ObjectSchemaReader that can read type infos from .net DLL
- **New**: Separation for settings. Added new property at `appsettings.json` called *TemplateSttings* where you can specify path for template configurations.

## 2.6.1021.422

- Big internal refactors.
- New attribute for properties of entities:
  - *HtmlEncode*: encode special chars
- Rename attribute *Empty* to *IsEmpty*
- Update NUGET packages:
  - "Newtonsoft.Json" Version="13.0.1"
  - "NLog" Version="4.7.9"
  - "MahApps.Metro" Version="2.4.4"
- Removes Affixer and WordList
- Schemes / RenameColumns can be specified by table: *table_name.column_name=new_column_name*
- Refactors for GeneratorAdapter, extract git responsibility to GitService
- Added *ShowMessages*, *ShowMessageWhenExitCodeNotZero* and *ShowMessageWhenExitCodeZero* to *Command* element
- Removes bat files for git actions
- Added helpers And, Or and Equal
- Refactors form CommandService

## 2.6.421.227

- Refactors for ReplacementResult
- Refactors of MessagesViewModels
- Changed colors for pinned messages
- Fix lock when scrolling message panel
- Commands output are redirect to message panel
- Added *ShowWindow* to *Command* element

## 2.6.121.220

- Refactors with UnityContainer, preparing for DI
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

## 2.5.221.213

- Can add helpers to MustacheHelpers to define functions that will be rendered

## 2.5.121.212

- Stop and shows errors when rendering
- New attributes for properties of entities:
  - *IsNumeric*: Indicates if a property of the entity is numeric
  - *IsString*: Indicates if a property of the entity is string
  - *IsDateOrTime*: Indicates if a property of the entity is DateTime or TimeSpan
  - *Scale*: Number of decimals for a numeric property

## 2.4.721.205

- New options in *appSettings*:
  - `Encoding: utf8 | latin1`: indicates output codification, defaults to utf8
  - `EndOfLine: crlf | lf`: indicates ends of lines in output files, defaults to system default

## 2.4.621.203

- Fix end of line en output files

## 2.4.521.203

- *AddMonth* trims initial '0' from month number and don't put '.' at the start.
- *AddBuildNumber* adds formated string *yy.Mdd*
- Updating MahApps.Metro to 2.4.3
- Updating NLog to 4.7.7
- Updating MySql.Data to 8.0.23

## 2.4.420.1215

- Add new button on command bar top open powershell at the output folder
- Fix responsibility for command bar view model
- Fix table panel for error: when partial group of table/views are showed and select "All"

## 2.4.320.1213

- Add new button on command bar to open output with Visual Studio Code if it accessible via PATH environment variable

## 2.4.220.1212

- Migrate all projects to .net 5
- Do not erase directories that start with "." when generating code

## 2.4.120.1204

- Refactors for MainWindow: put command bar and tables/views panels in controls

## 2.3.1620.1118

- Fix appsettings.json monitoring

## 2.3.1520.1111

- Updating principal project (L2Data2CodeWPF) to .Net 5
- Updating MySql.Data 8.0.22
- Updating System.Management 5.0.0
- Updating MahApps.Metro 2.3.4

## 2.3.1420.1101

- Adjust visibility for toggle switch *GenerateOnlyJson* depending on data source (set to false when json or fake data source)

## 2.3.1320.1101

- When code is generated, a *git add -A* is executed

## 2.3.1220.1030

- New setting **JsonGeneratedPath**: Indicates where the JSON file wil be generated when **generateJsonInfo** is *true*
- New option on UI to generate **only** JSON file.

## 2.3.1120.1019

- Updating NLog to 4.7.5 from 4.7.3
- Updating MahApps.Metro.IconPacks.Material to 4.6.0 from 4.4.0
- Updating MahApps.Metro.IconPacks.SimpleIcons to 4.6.0 from 4.4.0

## 2.3.1020.1012

- When defining the schema, it is possible to override the types of the columns, for example, in Sqlite, the *decimal* type in c# is cast to TEXT. With this new version, it is possible to set *decimal* to become NUMERIC.
- Added properties:
    - *OverrideDbType* (string): Overwritten type to be used in the output DB.
    - *DbTypeOverrided* (bool): Indicates whether the column type has been overridden.
- SchemaFactory provides a new provider but only for output data sources: sqlite

## 2.3.920.1010

- Fixed duplicated table names

## 2.3.820.1007

- Two different areas can use same area name but different data sources and output data sources.
```json
    "Areas": {
        "Area 1": {
            "Name": "AreaName",
            "DataSource": "datasource_1",
            "OutputDataSource": "output_datasource_1"
        },
        "Area 2": {
            "Name": "AreaName",
            "DataSource": "datasource_2",
            "OutputDataSource": "output_datasource_3"
        }
    }

```

## 2.3.720.926

- **appSettings.canCreateOutputDB** moved to **Schemas.connectionStringKey.CanCreateDb**
- Added properties to use in templates: *ColumnNameOrName* and *TableNameOrEntity*
- Added **Schemas.connectionStringKey.NormalizedNames**. When is true, then *ColumnNameOrName* will print *Name* and *TableNameOrEntity* will print *Entity*. When is false then *ColumnNameOrName* will print *ColumnName* and *TableNameOrEntity* will print *TableName*

## 2.3.620.925

- Added option **RenameColumns** at *Schemas.connectionString* to give option to rename some fields. Format:
```json
    "Schemas": {
        "connectionStringKey": {
            "RenameColumns": "columnName1=newColumnName1;columnName2=newColumnName2..."
        }
    }

```

## 2.3.520.925

- Add new bool property *CanCreateDB* that will be true when *DataSource* and *OutputDataSource* in Areas -> area are different and **appSettings.canCreateOutputDB** is true.

## 2.3.4.828

- Fix on first time execution, sometimes list of tables and views are empty.

## 2.3.3.828

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

## 2.3.2.827

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


## 2.3.1.820

- Added new option to generate code on different connection string. Actually, code is generated using input connection string (or Sqlite when fake or json is used), but, what about if we want to generate code using another connection string? Perhaps can update applicacion to specify output connection string and that connection will be used when generating code. At *appsettings > Areas > area* could be specified input and output connection strings separated by comma.

```json
    "Areas": {
        "area": "connectionString, connectionStringToDescriptions, outputConnectionString"
    },
```


## 2.3.0

- Added new schema reader **JsonSchemaReader** that enable application to read info from a JSON file. Set *appsettings > ConnectionStrings > connectionString* to the path of JSON file, *appsettings > Providers > connectionString* to *System.Data.JsonClient* and add an *appsettings > Area > area* with that *connectionString*
- Update Nlog 4.7.3, MySql.Data 8.0.20, MahApps.Metro 2.2.0, MahApps.Metro.IconPacks.Material 4.4.0, MahApps.Metro.IconPacks.SimpleIcons 4.4.0
- Update System.Data.SqlClient 4.8.2
- Some minor problems with table order

## 2.2.19.805

- A json info for tables will be generated (at log file) when *generateJsonInfo* is set to true in appsettings.json > appSettings. These would be used for the future feature **Generate from json datasource**

## 2.2.18.802

- Button to show vars, now at toolbar
- Fix vars order interpretation
- Doesn't recompile vars when start generationg code if *GeneratorCode* recives *Vars*
- *appsettings > Vars* can contain initialization set of vars

## 2.2.17.801

- Vars window visibility depending on appSettings > *showVarsWindow*
- Set vars window modal
- Changed XAML so DataContext is design mode.

## 2.2.16.731

- Can show window with compiled vars

## 2.2.15.731

- Adjust sample values on **Sample** and **NextSample**

## 2.2.14.730

- **Bug**: StringsExtensions bugs with nulls

## 2.2.13.729

- Added new property for entity fields: **Sample** and **NextSample**

  - **Sample**: outputs *1*, *new Date(2020,1,1)*, *TimeSpan.FromSeconsd(1)* or *"SAMPLE TEXT"* depending on field type.
  - **NextSample**: outputs next one sample.

## 2.2.12.728

- **New feature**: Chained templates

## 2.2.11.727

- **Bug**: When starting application and cannot connect to DB, the application hang. There was a problem when locking object *AllMessages* and *runningProcesses*

## 2.2.10.727

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

## 2.1.4.716

- Show spin when loading data from data source (on selecting new area)

## 2.1.3.715

- Open VS button disabled when just open a solution
- Optimize ProcessManager: CPU consumption has been dropped 5%

## 2.1.2.713

- Change theme to Light.Orange
- Change checkboxes to toggle switch
- Rename of property Library (on CodeGeneratorDto) to Model

## 2.1.1.713

- When selecting a MySql source, views aren't showing (bug in select catalog).
- Adding a **new** field for entities: `IsUpdatable`. If an entity is from a table, `IsUpdatable` is always true.

## 2.1.0.711

- New UI version, L2 Data2Code now using WPF. Same funcionalities, but UI has a modern style.

## 2.0.6.629

- Added tool to convert templates from version 1 to 2 (English names).

## 2.0.5.628

- Big changes on ProcessManager to do some task is asynchrony.
- Fix some defects on appsettings.json
- Make detection of opened solutions and editors in the background.

## 2.0.4.625

- GitCommit is not working.

## 2.0.3.621

- Detection on change settings is not working properly.

## 2.0.3.619

- Can use fake connection in templates (var database=fake)
- Application icon changed

## 2.0.2.618

- Multilingual application: English and Spanish.

## 2.0.1.618

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


## 2.0.0.616

- An silent error was generated if application cannot connect to database.

## 2.0.0.615

- A tool is added to convert Template.xml into Template.json, when in the future, will be preferred to use JSON files instead of XML to define templates. At the moment, templates definitions are using XML.
- Use MySql.Data.MySqlClient instead of MySqlConnector, since it is incompatible when reading the schema.

## 2.0.0.614

- Stop using app.config to use appsettings.json
