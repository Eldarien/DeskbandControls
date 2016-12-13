using System;
using System.IO;
using System.Reflection;

namespace Deskband.Common
{
    internal static class AssemblyResolver
    {
        public static void Initialize()
        {
            // We need this not only for custom assemblies (which we may not use at all),
            // but also for ProprtyGrid's TypeConverter attributes to work in case they are
            // defined not in the same assemply as PropertyGrid itself. Weird, but it works.
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath))
                return null;

            return Assembly.LoadFrom(assemblyPath);
        }
    }
}