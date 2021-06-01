# L2 Data2Code new features

## Reverse engineering

Think about an application that gets a folder and convert it to templates. We will need this vars:

- Company
- Area
- Module
- Entity

## Remove fake provider from settings

Think about how to process global templates, templates that doesn't have a table associated.

## Remove UseAffixer

Think about how people can put names on tables and columns and how can be translated to entity names.

## Add an option to specify a set of tables visible in table list

**Published on 2.3.3.828:** Possibility to specify a set of tables and views on new properties *Tables* and *Views* on *Modules* section. It would be amazing if we could exclude a set of tables and/or views

```json
    "Modules": {
        "Northwind.ERP": {
            "Name": "ERP",
            "Tables": "Order.+|Inv.+"
        }
    }
```

## Define a different database connection on generated code

**Published on 2.3.1.820:** Actually, code is generated using input connection string (or Sqlite when fake or json is used), but, what about if we want to generate code using another connection string? Perhaps can update application to specify output connection string and that connection will be used when generating code. At *appsettings > Areas > area* could be specified input and output connection strings separated by comma.

```json
    "Areas": {
        "area": "connectionString, connectionStringToDescriptions, outputConnectionString"
    },
```

## Show layer "in process" in long operations

- **Published on 2.1.4.716:** When selecting a new area, show an animation while accessing the data source to read the structure, as the data source may have slow access.

## Show error messages

- **Published on 2.2.10.727:** Try to show info, warning or error message

## Types of solutions

- **Published on 2.2.10.727:** Add a property in Template that indicates the type or types of files that the solution contains with the following specification, for example "vs, *. sln"

    - first field, type of icon to display:
      - vs: Visual Studio
      - vsc: Visual Studio Code
      - nb: Apache Netbeans
      - ec: Eclipse
      - ij: IntelliJIdea
    - second field, path or extension specification to find:
      - `* .sln`
      - `nbproject / project.xml`
      - `.conf / myspecialfile.conf`
    - third field, specification of how to open it:
      - (unspecified): try to open the found file using the system.
      - `program arguments`: in arguments we can pass it` {file} `or` {directory} `and it will be replaced by the file or directory where the previous field specification was found.

## Unlink from Area and Module in code

- **Published on 2.2.10.727:** Remove link to Area.Module in code (solution file always searched at /{{Area}}.{{Module}}/{{Area}}.{{Module}}.sln file)