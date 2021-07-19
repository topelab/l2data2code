using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace L2Data2Code.SchemaReader.Object
{
    /// <summary>
    /// Schema reader for objects in assembly
    /// </summary>
    public class ObjectSchemaReader : Schema.SchemaReader
    {
        private readonly string connectionString;
        private readonly string assemblyPath;
        private readonly string nameSpace;
        private readonly List<string> nameSpacesCollection;
        private INameResolver resolver;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="options">Schema options</param>
        public ObjectSchemaReader(SchemaOptions options) : base(options.SummaryWriter)
        {
            connectionString = options.ConnectionString;
            var parts = connectionString.Split(';');
            assemblyPath = parts[0];
            if (parts.Length > 1)
            {
                nameSpace = parts[1];
            }
            if (!File.Exists(assemblyPath))
            {
                throw new Exception($"Assembly file {assemblyPath} doesn't exist");
            }
            nameSpacesCollection = new List<string>();
        }

        /// <summary>
        /// Read schema from an assembly specicfied in options
        /// </summary>
        /// <param name="options">Schema reader options</param>
        /// <returns>Collection of tables</returns>
        public override Tables ReadSchema(SchemaReaderOptions options)
        {
            resolver = options.NameResolver ?? new DefaultNameResolver();
            nameSpacesCollection.Clear();
            var tables = GetTables(nameSpace, options);
            return tables;
        }

        private IEnumerable<Type> GetTypesFromAssembly(string nameSpace = null)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            var types = assembly.GetTypes().Where(t => !t.Name.StartsWith("<") && (nameSpace == null || t.Namespace.StartsWith(nameSpace)));
            return types;
        }

        private Tables GetTables(string nameSpace, SchemaReaderOptions options)
        {
            IEnumerable<Type> types = GetTypesFromAssembly(nameSpace);
            Tables result = new();
            foreach (var type in types.Where(t => options.TableRegex == null || options.TableRegex.IsMatch(t.Name)))
            {
                Table table = CreateTableFromType(nameSpace, type, options.AlternativeDescriptions);
                result.Add(table.Name, table);
            }
            return result;
        }

        private Table CreateTableFromType(string nameSpace, Type type, Dictionary<string, string> alternativeDescriptions = null)
        {
            var typeName = GetFriendlyTypeName(type, nameSpace);

            Table table = new Table { ClassName = typeName, Name = typeName, CleanName = resolver.ResolveTableName(typeName) };
            table.Columns = LoadColumnsFromProperties(nameSpace, type, table, alternativeDescriptions);
            return table;
        }

        private List<Column> LoadColumnsFromProperties(string nameSpace, Type type, Table table, Dictionary<string, string> alternativeDescriptions = null)
        {
            List<Column> columns = new();
            foreach (var item in type.GetProperties().Where(p => CanBeCollectected(p.PropertyType)))
            {
                var typeName = GetFriendlyTypeName(item.PropertyType, nameSpace);
                Column column = new Column
                {
                    Table = table,
                    TableName = table.Name,
                    Name = item.Name,
                    PropertyName = resolver.ResolveColumnName(table.Name, item.Name),
                    PropertyType = typeName.Replace("Nullable<", string.Empty).Replace(">", string.Empty),
                    IsNullable = IsNullable(item.PropertyType),
                    IsNumeric = item.PropertyType.IsNumericType(),
                    Description = alternativeDescriptions != null && alternativeDescriptions.ContainsKey(table.Name) ? alternativeDescriptions[item.Name] : null
                };
                columns.Add(column);
            }
            return columns;
        }

        private bool IsNullable(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            bool isNullable = type.ToString().StartsWith("System.Nullable")
                || typeCode == TypeCode.Object
                || typeCode == TypeCode.String;

            return isNullable;
        }

        private bool CanBeCollectected(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            return typeCode != TypeCode.Empty && typeCode != TypeCode.DBNull;
        }

        private string GetFriendlyTypeName(Type t, string nameSpace)
        {
            string typeName;
            using (var provider = new CSharpCodeProvider())
            {
                var typeRef = new CodeTypeReference(t);
                typeName = provider.GetTypeOutput(typeRef);

                if (!string.IsNullOrWhiteSpace(nameSpace))
                {
                    string supNameSpace = nameSpace.Replace(nameSpace.Split('.').Last(), string.Empty);
                    if (typeName.StartsWith(supNameSpace))
                    {
                        typeName = typeName.Replace(typeName.Replace(typeName.Split('.').Last(), string.Empty), string.Empty);
                    }
                }

                typeName = typeName
                    .Replace("System.Collections.Generic.", string.Empty)
                    .Replace("System.Linq.Expressions.", string.Empty)
                    .Replace("System.Linq.", string.Empty)
                    .Replace("System.", string.Empty);
            }
            return typeName;
        }

    }
}
