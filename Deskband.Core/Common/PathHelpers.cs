using System;

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
    }
}
