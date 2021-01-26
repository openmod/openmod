using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using OpenMod.API;
using OpenMod.API.Plugins;
using OpenMod.NuGet;

namespace OpenMod.Core.Plugins.NuGet
{
    [OpenModInternal]
    public class NuGetPluginAssembliesSource : IPluginAssembliesSource, IDisposable
    {
        private readonly NuGetPackageManager m_NuGetPackageManager;

        public NuGetPluginAssembliesSource(NuGetPackageManager packageManager)
        {
            m_NuGetPackageManager = packageManager;
        }

        public virtual async Task<ICollection<Assembly>> LoadPluginAssembliesAsync()
        {
            var assemblies = new List<Assembly>();

            foreach (var nupkgFile in Directory.GetFiles(m_NuGetPackageManager.PackagesDirectory, "*.nupkg",
                SearchOption.AllDirectories))
            {
                try
                {
                    var nupkgAssemblies = await m_NuGetPackageManager.LoadAssembliesFromNuGetPackageAsync(nupkgFile);
                    assemblies.AddRange(nupkgAssemblies.Where(d =>
                        d.GetCustomAttribute<PluginMetadataAttribute>() != null));
                }
                catch (Exception ex)
                {
                    m_NuGetPackageManager.Logger.LogError($"Failed to load assemblies from nuget package: {nupkgFile}");
                    m_NuGetPackageManager.Logger.LogError(ex.ToString());
                }
            }

            return assemblies;
        }

        public async Task<NuGetInstallResult> InstallPackageAsync(string packageName, string? version = null, bool isPreRelease = false)
        {
            var exists = await m_NuGetPackageManager.IsPackageInstalledAsync(packageName);
            return await InstallOrUpdateAsync(packageName, version, isPreRelease, exists);
        }

        public Task<NuGetInstallResult> UpdatePackageAsync(string packageName, string? version = null, bool isPreRelease = false)
        {
            return InstallOrUpdateAsync(packageName, version, isPreRelease, true);
        }

        public async Task<bool> UninstallPackageAsync(string packageName)
        {
            var package = await m_NuGetPackageManager.GetLatestPackageIdentityAsync(packageName);
            if (package == null)
            {
                return false;
            }

            return await m_NuGetPackageManager.RemoveAsync(package);
        }

        public virtual Task<bool> IsPackageInstalledAsync(string packageName)
        {
            return m_NuGetPackageManager.IsPackageInstalledAsync(packageName);
        }

        private async Task<NuGetInstallResult> InstallOrUpdateAsync(string packageName, string? version = null, bool isPreRelease = false, bool isUpdate = false)
        {
            if (string.IsNullOrEmpty(packageName))
            {
                throw new ArgumentException(nameof(packageName));
            }

            var previousVersion = await m_NuGetPackageManager.GetLatestPackageIdentityAsync(packageName);

            if (isUpdate && previousVersion == null)
            {
                return new NuGetInstallResult(NuGetInstallCode.PackageOrVersionNotFound);
            }

            var package = await m_NuGetPackageManager.QueryPackageExactAsync(packageName, version, isPreRelease);
            if (package == null)
            {
                return new NuGetInstallResult(NuGetInstallCode.PackageOrVersionNotFound);
            }

            version ??= package.Identity.Version.OriginalVersion;

            var packageIdentity = new PackageIdentity(package.Identity.Id, new NuGetVersion(version));

            var result = await m_NuGetPackageManager.InstallAsync(packageIdentity, isPreRelease);
            if (result.Code != NuGetInstallCode.Success && result.Code != NuGetInstallCode.NoUpdatesFound)
            {
                return result;
            }

            if (previousVersion != null)
            {
                await m_NuGetPackageManager.RemoveAsync(previousVersion);
            }

            return result;
        }

        public void Dispose()
        {
            m_NuGetPackageManager.Dispose();
        }
    }
}