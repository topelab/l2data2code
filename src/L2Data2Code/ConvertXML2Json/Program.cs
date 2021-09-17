using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ConvertXML2Json
{
    class Program
    {
        static void Main(string[] args)
        {
            var xmlFile = args.Length > 0 ? args[0] : "Templates.xml";

            var xml = File.ReadAllText(xmlFile);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            var jsonFile = Path.ChangeExtension(xmlFile, "json");
            var jsonLines = JsonConvert.SerializeXmlNode(doc.ChildNodes[1], Newtonsoft.Json.Formatting.Indented, true).Split(Environment.NewLine);
            var json = new StringBuilder();

            foreach (var item in jsonLines)
            {
                if (item.Contains("@xmlns:xs"))
                {
                    continue;
                }

                if (item.Contains("\"@Vars\": "))
                {
                    var resul = ExtractVars("Vars", item.Trim());
                    json.Append(resul);
                    continue;
                }

                if (item.Contains("\"@FinalVars\": "))
                {
                    var result = ExtractVars("FinalVars", item.Trim());
                    json.Append(result);
                    continue;
                }

                json.AppendLine(item.Replace("\"@", "\""));
            }


            File.WriteAllText(jsonFile, json.ToString());
        }

        private static StringBuilder ExtractVars(string section, string item)
        {
            var conditionals = new StringBuilder();
            var json = new StringBuilder();
            var vars = item.Split($"\"@{section}\": ")[1];
            vars = vars.Trim(',').Trim('"').Replace("\\r\\n", string.Empty).Replace("\\t", string.Empty);
            var allVars = vars.Split(';', StringSplitOptions.RemoveEmptyEntries);
            json.AppendLine($"\t\t\"{section}\": {{");
            bool inConditional = false;
            var lastElem = allVars.Last();

            foreach (var elem in allVars)
            {
                var text = elem.Trim();
                if (!text.Contains("="))
                {
                    continue;
                }

                if (text.StartsWith("."))
                {
                    conditionals.Append($"; {text}");
                    continue;
                }

                CheckConditionals(conditionals, json, inConditional, false);

                inConditional = false;
                if (text.StartsWith("if "))
                {
                    var condition = string.Join('=', text.Split('=').Take(2));
                    var thenPart = string.Join('=', text.Split('=').Skip(2));
                    conditionals.Append($"\t\t\t\"{condition}\": \"{thenPart}");
                    inConditional = true;
                    continue;
                }

                var variable = text.Split('=')[0].Trim();
                var value = text.Split('=')[1].Trim();
                json.Append($"\t\t\t\"{variable}\": \"{value}\"");
                json.AppendLine(lastElem == elem ? string.Empty : ",");
            }

            CheckConditionals(conditionals, json, inConditional, true);
            json.Append("\t\t}");
            json.AppendLine(item.Last() == ',' ? "," : string.Empty);
            return json;
        }

        private static void CheckConditionals(StringBuilder conditionals, StringBuilder json, bool inConditional, bool atLastItem)
        {
            if (inConditional)
            {
                conditionals.Append('"');
                json.Append(conditionals);
                json.AppendLine(atLastItem ? string.Empty : ",");
                conditionals.Clear();
            }
        }
    }
}
