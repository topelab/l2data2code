using System;

namespace ConvertTemplates
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Trying to convert templates from version 1 to 2");
            Conversion.Run(args.Length > 0 ? args[0] : null);
        }
    }
}
