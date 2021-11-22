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
            var options = MustacheOptionsFactory.Create(args);
            IJsonSetting jsonSetting = new JsonSetting(options.JsonDataFile);
            IMustacheHelpers helpers = new MustacheHelpers();
            IMustacheRenderizer mustacher = new MustacheRenderizer(helpers);

            //string resul = mustacher.Render(options.TemplatePath, jsonSetting.Config);


        }
    }
}
