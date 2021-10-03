using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using L2Data2Code.SharedLib.Extensions;

namespace ConvertTemplates
{
    internal class Conversion
    {
        private static string[] templates;
        private static readonly Dictionary<string, string> conversions = new()
        {
            { "Entidad", "Entity" },
            { "Libreria", "Module" },
            { "NombreTabla", "TableName" },
            { "ListaIgnorados", "IgnoreColumns" },
            { "CamposIgnorados", "IgnoreColumns" },
            { "Parametros", "UnfilteredColumns" },
            { "GenerarBase", "GenerateBase" },
            { "SeGeneranReferencias", "GenerateReferences" },
            { "EsVista", "IsView" },
            { "Descripcion", "Description" },
            { "CadenaConexion", "ConnectionString" },
            { "ProveedorDatos", "DataProvider" },
            { "AtributosTodos", "AllColumns" },
            { "Atributos", "Columns" },
            { "AtributosPersistibles", "PersistedColumns" },
            { "AtributosFK", "ForeignKeyColumns" },
            { "Colecciones", "Collections" },
            { "HayColecciones", "HasCollections" },
            { "HayForeignKeys", "HasForeignKeys" },
            { "AtributosNotPrimaryKey", "NotPrimaryKeyColumns" },
            { "HayAtributosNoPK", "HasNotPrimaryKeyColumns" },
            { "HayAtributosPK", "HasPrimaryKeyColumns" },
            { "EntidadDebil", "IsWeakEntity" },
            { "Nombre", "Name" },
            { "UsarCastellano", "UseSpanish" },
            { "Id_o_Nombre", "IdOrName" },
            { "Tabla", "Table" },
            { "Tipo", "Type" },
            { "TipoNullable", "NullableType" },
            { "NombreColumna", "ColumnName" },
            { "NombreColumnaDiferente", "IsNameDifferentToColumnName" },
        };

        private static readonly Dictionary<string, string> conversionsExtensions = new()
        {
            { "Nombre", "Name" },
            { "UsarCastellano", "UseSpanish" },
            { "Id_o_Nombre", "IdOrName" },
            { "Tabla", "Table" },
            { "Tipo", "Type" },
            { "TipoNullable", "NullableType" },
            { "Descripcion", "Description" },
            { "NombreColumna", "ColumnName" },
            { "NombreColumnaDiferente", "IsNameDifferentToColumnName" },
        };

        private static readonly Dictionary<string, string> conversionsFiles = new()
        {
            { "Entidad", "Entity" },
            { "Libreria", "Module" },
        };

        internal static void Run(string path)
        {
            if (!ExistTemplate(path))
            {
                return;
            }

            ProcessTemplates();
        }

        private static bool ExistTemplate(string path)
        {
            string currentPath = path ?? Directory.GetCurrentDirectory();
            templates = Directory.GetFiles(currentPath, "Templates.xml", SearchOption.AllDirectories);
            return templates.Length > 0;
        }

        private static void ProcessTemplates()
        {
            foreach (var item in templates)
            {
                ProcessTemplate(item);
            }
        }

        private static void ProcessTemplate(string item)
        {
            ChangeTemplate(item);
            ChangeDirectories(Path.GetDirectoryName(item));
            ChangeFiles(item);
        }

        private static void ChangeTemplate(string item)
        {
            string content = File.ReadAllText(item);

            content = content
                .Replace("Libreria=", "Module=")
                .Replace("ListaIgnorados=", "IgnoreColumns=")
                .Replace("CamposIgnorados=", "IgnoreColumns=")
                .Replace("{{Libreria}}", "{{Module}}")
                .Replace("{{Entidad}}", "{{Entity}}");

            File.WriteAllText(item, content);
        }

        private static void ChangeDirectories(string path)
        {
            var paths = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var item in paths)
            {
                var newPath = DoReplaceFileName(item);
                if (!newPath.Equals(item))
                {
                    Directory.Move(item, newPath);
                    ChangeDirectories(newPath);
                }
                else
                {
                    ChangeDirectories(item);
                }
            }
        }

        private static string DoReplaceFileName(string fileName)
        {
            var result = fileName;
            foreach (var item in conversionsFiles)
            {
                var regex = new Regex(@"\b" + item.Key + @"\b", RegexOptions.Singleline);
                result = regex.Replace(result, item.Value);
            }
            return result;
        }

        private static void ChangeFiles(string item)
        {
            string path = Path.GetDirectoryName(item).AddPathSeparator();
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                ChangeFile(file);
            }
        }

        private static void ChangeFile(string file)
        {
            string newName = DoReplaceFileName(file);
            var content = File.ReadAllText(file);
            var result = DoReplacement(content);
            if (!newName.Equals(file))
            {
                File.Delete(file);
                file = newName;
            }
            File.WriteAllText(file, result);
        }

        private static string DoReplacement(string content)
        {
            var searchBase = @"(\{\{(#|\^|/){0,1})";
            var replaceBase = @"$1";
            var result = content;
            foreach (var item in conversions)
            {
                var regex = new Regex(searchBase + item.Key + @"\b", RegexOptions.Singleline);
                result = regex.Replace(result, replaceBase + item.Value);
            }
            foreach (var item in conversionsExtensions)
            {
                var regex = new Regex(@"\.\b" + item.Key + @"\b", RegexOptions.Singleline);
                result = regex.Replace(result, "." + item.Value);
            }
            return result;
        }
    }
}