using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using OpenMod.NuGet.Persistence;
using VYaml.Parser;
using VYaml.Serialization;

namespace OpenMod.NuGet
{
    public class PackagesDataStore
    {
        private readonly string m_Path;
        private readonly YamlSerializerOptions m_YamlOptions;
        public ILogger? Logger { get; set; }

        public PackagesDataStore(string path)
        {
            m_Path = path;
            m_YamlOptions = new YamlSerializerOptions
            {
                Resolver = CompositeResolver.Create([YamlHashSetTypeFormatter<SerializedNuGetPackage>.Instance], StandardResolver.DefaultResolvers)
            };
        }

        public async Task AddOrUpdatePackageIdentity(PackageIdentity id)
        {
            var file = await ReadPackagesFiles();
            var package = file.Packages?.FirstOrDefault(d => d.Id.Equals(id.Id));

            if (package != null)
            {
                // update version
                package.Version = ConvertNugetVersion(id.Version);
            }
            else
            {
                // add new entry
                package = new SerializedNuGetPackage
                {
                    Id = id.Id,
                    Version = ConvertNugetVersion(id.Version)
                };

                file.Packages ??= [];
                file.Packages.Add(package);
            }

            await WritePackagesFile(file);
        }

        public async Task<bool> RemovePackageAsync(string packageId)
        {
            if (!await ExistsAsync())
            {
                return false;
            }

            var file = await ReadPackagesFiles();

            // ReSharper disable once UseNullPropagation
            if (file.Packages == null)
            {
                return false;
            }

            var package = file.Packages.FirstOrDefault(d => d.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase));
            if (package == null || !file.Packages.Remove(package))
            {
                // package does not exist or could not be removed
                return false;
            }

            await WritePackagesFile(file);
            return true;
        }

        public async Task EnsureExistsAsync()
        {
            if (!await ExistsAsync())
            {
                await WritePackagesFile(GetDefaultFile());
            }
        }

        private Task<bool> ExistsAsync()
        {
            return Task.FromResult(File.Exists(m_Path));
        }

        public async Task<ICollection<PackageIdentity>> GetPackagesAsync()
        {
            var file = await ReadPackagesFiles();
            if (file.Packages == null)
            {
                return [];
            }

            return file.Packages
                .Select(d => new PackageIdentity(d.Id, ConvertOpenModVersion(d.Version)))
                .ToList();
        }

        private async Task<SerializedPackagesFile> ReadPackagesFiles()
        {
            await EnsureExistsAsync();

            await using var fileStream = File.OpenRead(m_Path);
            if (fileStream.Length == 0)
            {
                return GetDefaultFile();
            }

            try
            {
                var result = await YamlSerializer.DeserializeAsync<SerializedPackagesFile>(fileStream, m_YamlOptions);
                result.Packages ??= [];
                return result;
            }
            catch(YamlParserException ex)
            {
                Logger?.LogError($"Fail to deserialize packages.yaml: {ex}");
                var file = GetDefaultFile();
                await WritePackagesFile(file);
                return file;
            }
        }

        private Task WritePackagesFile(SerializedPackagesFile file)
        {
            var yaml = YamlSerializer.SerializeToString(file, m_YamlOptions);
            File.WriteAllText(m_Path, yaml, Encoding.UTF8);
            return Task.CompletedTask;
        }

        private static SerializedPackagesFile GetDefaultFile()
        {
            return new()
            {
                Packages = []
            };
        }

        private static NuGetVersion? ConvertOpenModVersion(string version)
        {
            return !version.Equals("latest", StringComparison.OrdinalIgnoreCase)
                ? new NuGetVersion(version)
                : null;
        }

        private static string ConvertNugetVersion(NuGetVersion? version)
        {
            return version?.OriginalVersion ?? "latest";
        }
    }
}