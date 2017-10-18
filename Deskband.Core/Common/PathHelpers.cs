using System;
using System.IO;

namespace Deskband.Core.Common
{
    public static class PathHelpers
    {
        public static string TryPlaceEnvVars(string path)
        {
            if (path == null) return null;

            var appData = Environment.GetEnvironmentVariable("APPDATA");
            if (path.StartsWith(appData, StringComparison.OrdinalIgnoreCase))
            {
                path = $"%APPDATA%{path.Substring(appData.Length)}";
            }
            return path;
        }

        public static string ResolvePath(string path, string rootPath)
        {
            if (path.StartsWith("..", StringComparison.OrdinalIgnoreCase))
            {
                path = Path.Combine(rootPath, path);
            }
            return Environment.ExpandEnvironmentVariables(path);
        }
    }
}
