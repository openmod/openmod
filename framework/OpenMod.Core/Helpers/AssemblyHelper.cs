using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMod.Core.Helpers
{
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
                var regex = new Regex(Regex.Escape(assembly.GetName().Name + "."));
                var fileName = regex.Replace(resourceName, string.Empty, 1);

                var filePath = Path.Combine(baseDir, fileName);
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream ?? throw new MissingManifestResourceException($"Couldn't find resource: {resourceName}"));
                var fileContent = reader.ReadToEnd();
                
                if (File.Exists(filePath))
                {
                    if (overwrite)
                    {
                        File.WriteAllText(filePath, fileContent);
                        continue;
                    }
                    
                    UpdateResource(filePath, fileContent);
                    continue;
                }
                
                File.WriteAllText(filePath, fileContent);
            }
        }

        private static void UpdateResource(string filePath, string fileContent)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();;
                    
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
                    
            var encodedData = File.ReadAllBytes(filePath);
            var serializedYaml = Encoding.UTF8.GetString(encodedData);
                    
            var diskResource = deserializer.Deserialize<Dictionary<string, string>>(serializedYaml);
            var newResource = deserializer.Deserialize<Dictionary<string, string>>(fileContent);

            bool missingKey = false;
            foreach (var key in newResource.Keys)
            {
                if (diskResource.ContainsKey(key))
                {
                    continue;
                }

                missingKey = true;
                diskResource.Add(key, newResource[key]);
            }

            if (!missingKey)
            {
                return;
            }

            serializedYaml = serializer.Serialize(diskResource);
            encodedData = Encoding.UTF8.GetBytes(serializedYaml);
            File.WriteAllBytes(filePath, encodedData);
        }
    }
}