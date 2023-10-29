using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using OpenMod.API;
using OpenMod.Common.Hotloading;
using Serilog;

namespace OpenMod.Core.Helpers
{
    [OpenModInternal]
    public static class AssemblyHelper
    {
        private static readonly string[] s_ExcludedResources = new[]
        {
            "packages.yaml"
        };

        public static void CopyAssemblyResources(Assembly assembly, string? baseDir, bool overwrite = false)
        {
            baseDir ??= string.Empty;

            var resourceNames = assembly.GetManifestResourceNames();
            if (resourceNames.Length > 0 && !Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }

            var assemblyName = Hotloader.GetRealAssemblyName(assembly);
            var assemblyNameDot = $"{assemblyName.Name}.";
            var assemblyNameDotRegex = new Regex(Regex.Escape(assemblyNameDot));

            Log.Debug("Found {Length} resources for {AssemblyName}.", resourceNames.Length, assemblyName.Name);

            foreach (var resourceName in resourceNames)
            {
                if (resourceName.EndsWith("..directory"))
                {
                    continue;
                }

                if (!resourceName.Contains(assemblyNameDot))
                {
                    Log.Warning(
                        "{ResourceName} does not contain assembly name in assembly: {AssemblyName}. <AssemblyName> and <RootNamespace> must be equal inside your plugins .csproj file",
                        resourceName, assemblyName.Name);
                }

                var fileName = assemblyNameDotRegex.Replace(resourceName, string.Empty, 1);
                if (s_ExcludedResources.Contains(fileName))
                {
                    continue;
                }

                Log.Debug("Working on {ResourceName} resource with name {FileName}.", resourceName, fileName);

                var fileNameParts = fileName.Split('.');

                var fileNameSb = new StringBuilder();
                var pathSb = new StringBuilder(assemblyNameDot);

                foreach (var part in fileNameParts)
                {
                    var partDot = $"{part}.";
                    pathSb.Append(partDot);

                    using var tmpStream = assembly.GetManifestResourceStream(pathSb + ".directory");
                    var isDirectory = tmpStream != null;
                    fileNameSb.Append(isDirectory ? $"{part}{Path.DirectorySeparatorChar}" : partDot);
                }

                fileName = fileNameSb[^1] == '.' ? fileNameSb.ToString(0, fileNameSb.Length - 1) : fileNameSb.ToString();

                Log.Debug("Resource {ResourceName} with name {FileName} after checks.", resourceName, fileName);
                var directory = Path.GetDirectoryName(fileName);
                if (directory != null)
                {
                    directory = Path.Combine(baseDir, directory);
                }

                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var filePath = Path.Combine(baseDir, fileName);
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream ??
                                                    throw new MissingManifestResourceException(
                                                        $"Couldn't find resource: {resourceName}"));
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