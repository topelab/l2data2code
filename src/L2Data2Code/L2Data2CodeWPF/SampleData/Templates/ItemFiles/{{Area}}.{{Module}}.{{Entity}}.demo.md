# {{ApplicationName}}.{{TableName}}

This is {{ApplicationTitle}} version {{Version}} using table {{TableName}}

## Variables

Company: {{Company}}
Area: {{Area}}
Libreria: {{Module}}
NombreTabla: {{TableName}}
Entidad: {{Entity}}

Usando "Vars.Variable" ({{Vars.Count}})
{{#Vars.Keys}}{{.}} = {{GetVar .}}
{{/Vars.Keys}}

{{#SetDomain}}
Generar Domain
{{/SetDomain}}
{{#SetUseCases}}
Generar SetUseCases
{{/SetUseCases}}
{{#SetAdapters}}
Generar SetAdapters
{{/SetAdapters}}
{{#SetWebApiControllers}}
Generar SetWebApiControllers
{{/SetWebApiControllers}}
{{#SetWebApi}}
Generar WebApi
{{/SetWebApi}}

# {{Vars.ApplicationTitle}}

## Table {{TableName}} -> Entity {{Entity}}

### Entity functions
Entidad.Camelize: {{Entity.Camelize}}
Entidad.PluralCamelize: {{Entity.PluralCamelize}}
Entidad.Camelize.ToPlural: {{Entity.Camelize.ToPlural}}
Entidad.SingularCamelize: {{Entity.SingularCamelize}}
Entidad.Pascalize: {{Entity.Pascalize}}
Entidad.ToPlural: {{Entity.ToPlural}}
Entidad.ToSingular: {{Entity.ToSingular}}
Entidad.HumanizeUnCapitalize: {{Entity.HumanizeUnCapitalize}}
Entidad.Humanize: {{Entity.Humanize}}

### Other functions

- AddBuildNumber: Version.AddBuildNumber = {{Version.AddBuildNumber}}
- IsTrue: SetDomain.IsTrue = {{SetDomain.IsTrue}}
- DoubleSlash: ConnectionString.DoubleSlash = {{ConnectionString.DoubleSlash}}

### Index
{{#Entity.MultiplePKColumns}}Índice compuesto: {{#PrimaryKeys}}{{Type}} {{Name.Camelize}}{{^IsLast}}, {{/IsLast}}{{/PrimaryKeys}}
{{/Entity.MultiplePKColumns}}{{#PrimaryKeys}}Nombre columna: {{ColumnName}} -> {{Name}}
{{#Description.NotEmpty}}Descripción: {{Description}}
{{/Description.NotEmpty}}Tipo: {{Type}}  MaxLenght: {{Precision}}
Orden:{{PkOrder}}

{{/PrimaryKeys}}
{{#HasNotPrimaryKeyColumns}}### Columnas{{/HasNotPrimaryKeyColumns}}
{{#NotPrimaryKeys}}
Nombre columna: {{ColumnName}} -> {{Name}}
{{#Description.NotEmpty}}Descripción: {{Description}}
{{/Description.NotEmpty}}{{^Description.NotEmpty}}Descripción vacía: {{Name.Humanize}}
{{/Description.NotEmpty}}Tipo: {{Type}}  MaxLenght: {{Precision}}
IsString: {{#IsString}}Sí{{/IsString}}
IsNumeric: {{#IsNumeric}}Sí
Scale: {{Scale}}{{/IsNumeric}}
IsDateOrTime: {{#IsDateOrTime}}Sí{{/IsDateOrTime}}
Nullable: {{#Nullable}}Sí{{/Nullable}}


{{/NotPrimaryKeys}}
