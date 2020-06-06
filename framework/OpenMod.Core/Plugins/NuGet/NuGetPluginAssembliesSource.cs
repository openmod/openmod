using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using OpenMod.NuGet;

namespace OpenMod.Core.Plugins.NuGet
{
    public class NuGetPluginAssembliesSource : PluginAssembliesSource, IDisposable
    {
        public NuGetPackageManager NuGetPackageManager { get; }
        private readonly string m_PackagesDirectory;

        public NuGetPluginAssembliesSource(string packagesDirectory)
        {
            m_PackagesDirectory = packagesDirectory;

            var adapter = new NuGetSerilogLogger();
            NuGetPackageManager = new NuGetPackageManager(adapter, packagesDirectory);
            NuGetPackageManager.InstallAssemblyResolver();
        }

        public override async Task<ICollection<Assembly>> LoadPluginAssembliesAsync()
        {
            List<Assembly> assemblies = new List<Assembly>();
            
            foreach (var nupkgFile in Directory.GetFiles(m_PackagesDirectory, "*.nupkg"))
            {
                var nupkgAssemblies = await NuGetPackageManager.LoadAssembliesFromNuGetPackageAsync(nupkgFile);
                assemblies.AddRange(nupkgAssemblies.Where(d => d.GetCustomAttribute<PluginMetadataAttribute>() != null));
            }

            return assemblies;
        }

        public async Task<NuGetInstallResult> InstallPackageAsync(string packageName, string version = null, bool isPreRelease = false)
        {
            bool exists = await NuGetPackageManager.IsPackageInstalledAsync(packageName);
            return await InstallOrUpdateAsync(packageName, version, isPreRelease, exists);
        }

        public Task<NuGetInstallResult> UpdatePackageAsync(string packageName, string version = null, bool isPreRelease = false)
        {
            return InstallOrUpdateAsync(packageName, version, isPreRelease, true);
        }

        public async Task<bool> UninstallPackageAsync(string packageName)
        {
            var package = await NuGetPackageManager.GetLatestPackageIdentityAsync(packageName);
            if (package == null)
            {
                return false;
            }

            return await NuGetPackageManager.RemoveAsync(package);
        }

        public virtual async Task<bool> IsPackageInstalledAsync(string packageName)
        {
            return await NuGetPackageManager.IsPackageInstalledAsync(packageName);
        }

        private async Task<NuGetInstallResult> InstallOrUpdateAsync(string packageName, string version = null, bool isPreRelease = false, bool isUpdate = false)
        {
            PackageIdentity previousVersion = await NuGetPackageManager.GetLatestPackageIdentityAsync(packageName);

            if (isUpdate && previousVersion == null)
            {
                return new NuGetInstallResult(NuGetInstallCode.PackageOrVersionNotFound);
            }

            var package = (await NuGetPackageManager.QueryPackageExactAsync(packageName, version, isPreRelease));
            if (package == null)
            {
                return new NuGetInstallResult(NuGetInstallCode.PackageOrVersionNotFound);
            }

            if (version == null)
            {
                version = package.Identity.Version.OriginalVersion; // set to latest version
            }

            var packageIdentity = new PackageIdentity(package.Identity.Id, new NuGetVersion(version));

            var result = await NuGetPackageManager.InstallAsync(packageIdentity, isPreRelease);
            if (result.Code != NuGetInstallCode.Success)
            {
                return result;
            }

            if (isUpdate)
            {
                await NuGetPackageManager.RemoveAsync(previousVersion);
            }
            return result;
        }

        public void Dispose()
        {
            NuGetPackageManager?.Dispose();
        }
    }
}