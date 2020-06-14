using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenMod.Core.Helpers
{
    public static class AssemblyHelper
    {
        public static void CopyAssemblyResources(Assembly assembly, string baseDir, bool overwrite = false)
        {
            baseDir ??= string.Empty;

            var resourceNames = assembly.GetManifestResourceNames();
            foreach (var resourceName in resourceNames)
            {
                var regex = new Regex(Regex.Escape(assembly.GetName().Name + "."));
                var fileName = regex.Replace(resourceName, string.Empty, 1);

                var filePath = Path.Combine(baseDir, fileName);
                if (File.Exists(filePath) && !overwrite)
                {
                    continue;
                }

                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream ?? throw new InvalidOperationException());

                var fileContent = reader.ReadToEnd();
                File.WriteAllText(filePath, fileContent);
            }
        }
    }
}