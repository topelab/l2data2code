using System;
using System.IO;

namespace L2Data2Code.SharedLib.Extensions
{
    public static class DirectoryExtensions
    {
        public static T GetResultUsingBasePath<T>(this string path, Func<T> func)
        {
            T result;
            var currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(path ?? currentDirectory);
            result = func();
            Directory.SetCurrentDirectory(currentDirectory);
            return result;
        }
    }
}
