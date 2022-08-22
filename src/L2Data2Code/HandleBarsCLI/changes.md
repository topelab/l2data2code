### 1.3.2

- **New**: HandleBarsCLI will search into "partials" path to get `*.partial` files.
- Putting helpers into CustomHelpers.
- Removed references to Stubble.
- Removed Mustache CLI utility. HandleBarsCLI replace it and its functionalities.

### 1.3.0

- **New**: Added mustache renderer Handlebars.Net. Stubble will be replaced with Handlebars.
- **New**: Added new CLI tool HandleBarsCLI that will replace Mustache tool previously created.

### 1.2.1

- Generated JSON redesign
- Added to MustacheHelpers *Join* with three variants:
  - {{Join separator}}: Takes an array a join with a separator.
  - {{JoinWithHeader separator header}}: Takes an array a join with a separator starting with a header.
  - {{JoinWithHeaderFooter separator header footer}}: Takes an array a join with a separator starting with a header and ending with a footer.
  - *separator, header and footer* are strings
- Mustache add *IsFirst* and *IsLast* to nodes at JObject[] objects
- Upgrade NLog to 5.0.1 from 5.0.0

### 1.2.0

- Upgrade to Topelab.Core 1.4.0
- Changed version numbering

### 1.1.5

- Connection strings for JsonClient and ObjectClient providers are now relative to template path
- **New**: Added new var *TemplatePath*

### 1.1.4

- Update NuGet packages.

### 1.1.3

- **New**: Added extension *DoubleSlash* to double back slash inside quote string


### 1.1.2

- Fix functionality for GenerateOnlyJson
- Upgrade NLog

### 1.1.1

- **Breaking change**: "Areas" section needs to be renamed to "DataSources", and any "Name" element inside, needs to be renamed to "Area".

### 1.0.2

- Update NuGet packages.
- Adapted to version 1.3.1122.121 of Topelab.Core.Resolver

### 1.0.1

- **New**: Refactors to use Topelab.Core.Resolver (not in NUGET public repository, need to be copied local)
- **New**: Refactors and upgrade to .Net 6
- **New**: *Templates.Template.SavePath* can use name of template using var *Template*

### 1.0.0

- **New**: Mustache CLI, a command line utility to execute a file name and content transformation (based on "mustache") in tree of paths for every path and file.

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


