using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json.Serialization;
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
                    var conditionals = new StringBuilder();
                    var vars = item.Split("\"@Vars\": ")[1];
                    vars = vars.Trim('"').Replace("\\r\\n", string.Empty).Replace("\\t", string.Empty);
                    var allVars = vars.Split(';');
                    json.AppendLine("\t\t\"Vars\": {");
                    bool inConditional = false;

                    foreach (var elem in allVars)
                    {
                        var text = elem.Trim();
                        if (!text.Contains("="))
                        {
                            continue;
                        }

                        if (text.StartsWith("."))
                        {
                            text = text.Trim('.');
                            var thenPart = "\"" + text.Replace("=", "\": \"") + "\",";
                            conditionals.AppendLine($"\t\t\t\t\t{thenPart}");
                            continue;
                        }
                        else
                        {
                            if (inConditional)
                            {
                                conditionals.AppendLine("\t\t\t\t},\n\t\t\t},");
                            }
                            inConditional = false;
                        }

                        if (text.StartsWith("if "))
                        {
                            var condition = text.Split(' ')[1];
                            var thenPart = "\"" + string.Join(' ', text.Split(' ').Skip(2)).Replace("=", "\": \"") + "\",";
                            inConditional = true;
                            conditionals.AppendLine($"\t\t\t{{\n\t\t\t\t\"if\": \"{condition}\",");
                            conditionals.AppendLine($"\t\t\t\t\"then\": {{");
                            conditionals.AppendLine($"\t\t\t\t\t{thenPart}");
                            continue;
                        }

                        var variable = text.Split('=')[0].Trim();
                        var value = text.Split('=')[1].Trim();
                        json.AppendLine($"\t\t\t\"{variable}\": \"{value}\",");
                    }
                    json.AppendLine("\t\t},");
                    if (inConditional)
                    {
                        conditionals.AppendLine("\t\t\t\t},\n\t\t\t},");
                    }
                    json.AppendLine("\t\t\"Conditions\": [");
                    json.Append(conditionals);
                    json.AppendLine("\t\t]");
                }
                else
                {
                    json.AppendLine(item.Replace("\"@", "\""));
                }
            }


            File.WriteAllText(jsonFile, json.ToString());
        }
    }
}
