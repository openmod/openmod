using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMod.NuGet
{
    public class PackagesDataStore
    {
        private readonly string m_Path;
        private readonly ISerializer m_Serializer;
        private readonly IDeserializer m_Deserializer;

        public PackagesDataStore(string path)
        {
            m_Path = path;
            m_Serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            m_Deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
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

                file.Packages ??= new HashSet<SerializedNuGetPackage>();
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
                return new List<PackageIdentity>();
            }

            return file.Packages
                .Select(d => new PackageIdentity(d.Id, ConvertOpenModVersion(d.Version)))
                .ToList();
        }

        private async Task<SerializedPackagesFile> ReadPackagesFiles()
        {
            await EnsureExistsAsync();

            var yaml = File.ReadAllText(m_Path);
            var result = m_Deserializer.Deserialize<SerializedPackagesFile>(yaml);
            result.Packages ??= new HashSet<SerializedNuGetPackage>();
            return result;
        }

        private Task WritePackagesFile(SerializedPackagesFile file)
        {
            var yaml = m_Serializer.Serialize(file);
            File.WriteAllText(m_Path, yaml, Encoding.UTF8);
            return Task.CompletedTask;
        }

        private SerializedPackagesFile GetDefaultFile()
        {
            return new()
            {
                Packages = new HashSet<SerializedNuGetPackage>()
            };
        }

        private NuGetVersion? ConvertOpenModVersion(string version)
        {
            return !version.Equals("latest", StringComparison.OrdinalIgnoreCase)
                ? new NuGetVersion(version)
                : null;
        }

        private string ConvertNugetVersion(NuGetVersion? version)
        {
            return version?.OriginalVersion ?? "latest";
        }
    }
}