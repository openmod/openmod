using System.IO;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using OpenMod.API;
using OpenMod.Core.Hotloading;
using Serilog;

namespace OpenMod.Core.Helpers
{
    [OpenModInternal]
    public static class AssemblyHelper
    {
        public static void CopyAssemblyResources(Assembly assembly, string baseDir, bool overwrite = false)
        {
            baseDir ??= string.Empty;

            var resourceNames = assembly.GetManifestResourceNames();

            if (resourceNames.Length > 0 && !Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }

            foreach (var resourceName in resourceNames)
            {
                var assemblyName = Hotloader.GetRealName(assembly);

                if (!resourceName.Contains(assemblyName.Name + "."))
                {
                    Log.Warning($"{resourceName} does not contain assembly name in assembly: {assemblyName.Name}. <AssemblyName> and <RootNamespace> must be equal inside your plugins .csproj file.");
                }

                var regex = new Regex(Regex.Escape(assemblyName.Name + "."));
                var fileName = regex.Replace(resourceName, string.Empty, 1);

                var filePath = Path.Combine(baseDir, fileName);
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream ?? throw new MissingManifestResourceException($"Couldn't find resource: {resourceName}"));
                var fileContent = reader.ReadToEnd();
                
                if (File.Exists(filePath) && !overwrite)
                {
                    continue;
                }
                
                File.WriteAllText(filePath, fileContent);
            }
        }
    }
}