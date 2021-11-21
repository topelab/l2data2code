using L2Data2Code.BaseMustache.Extensions;
using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.BaseMustache.Services;
using L2Data2Code.SharedLib.Helpers;
using System;

namespace Mustache
{
    class Program
    {
        static void Main(string[] args)
        {
            IJsonSetting jsonSetting = new JsonSetting(@"C:\arc\src\LinqPad\efcore\data.json");
            IMustacheHelpers helpers = new MustacheHelpers();
            IMustacheRenderizer mustacher = new MustacheRenderizer(helpers);

            string resul = mustacher.Render(@"
---------
Hello!
    
    {{#Dishes}}
{{#Valid}}
    {{Type.ToPlural}} - {{Dish.UnCapitalize}}
{{/Valid}}
    {{/Dishes}}
---------
", jsonSetting.Config);

            Console.WriteLine(resul);

        }
    }
}
