using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Plugins;
using OpenMod.NuGet;
using Semver;

namespace OpenMod.Core.Plugins
{
    [OpenModInternal]
    public class PluginAssemblyStore : IPluginAssemblyStore, IDisposable
    {
        private readonly ILogger<PluginAssemblyStore> m_Logger;
        private readonly NuGetPackageManager m_NuGetPackageManager;

        public PluginAssemblyStore(ILogger<PluginAssemblyStore> logger, NuGetPackageManager nuGetPackageManager)
        {
            m_Logger = logger;
            m_NuGetPackageManager = nuGetPackageManager;
        }

        private readonly List<WeakReference> m_LoadedPluginAssemblies = new List<WeakReference>();
        public IReadOnlyCollection<Assembly> LoadedPluginAssemblies
        {
            get
            {
                return m_LoadedPluginAssemblies
                    .Where(d => d.IsAlive)
                    .Select(d => d.Target)
                    .Cast<Assembly>()
                    .ToList();
            }
        }

        public IConfigurationRoot Configuration;

        public async Task<ICollection<Assembly>> LoadPluginAssembliesAsync(IPluginAssembliesSource source)
        {
            var providerAssemblies = await source.LoadPluginAssembliesAsync();

            foreach (var providerAssembly in providerAssemblies.ToList())
            {
                var pluginMetadata = providerAssembly.GetCustomAttribute<PluginMetadataAttribute>();
                if (pluginMetadata == null)
                {
                    m_Logger.LogWarning($"No plugin metadata attribute found in assembly: {providerAssembly}; skipping loading of this assembly as plugin");
                    providerAssemblies.Remove(providerAssembly);
                    continue;
                }

                ICollection<Type> types;
                try
                {
                    types = providerAssembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    //Remove Assembly from loading because required dependencies are missing.
                    providerAssemblies.Remove(providerAssembly);

                    var missingAssemblies = CheckRequiredDependencies(ex.LoaderExceptions);
                    var installMissingDependencies = Configuration != null &&
                                                     Configuration.GetSection("nuget:tryAutoInstallMissingDependencies")
                                                         .Get<bool>();//todo fix Configuration always null

                    if (!installMissingDependencies)
                    {
                        m_Logger.LogWarning($"Couldn't load plugin from {providerAssembly}: Failed to resolve required dependencies: {string.Join(", ", missingAssemblies.Keys)}", Color.DarkRed);
                        continue;
                    }

                    await TryInstallRequiredDependenciesAsync(providerAssembly, missingAssemblies);
                    //var installWithSuccess = await TryInstallRequiredDependenciesAsync(providerAssembly, missingAssemblies);
                    /*if (installWithSuccess)
                    {
                        assembliesWithMissingDependencies.Add(providerAssembly);
                    }*/

                    continue;
                }

                if (types.Any(d => typeof(IOpenModPlugin).IsAssignableFrom(d) && !d.IsAbstract && d.IsClass)) 
                    continue;

                m_Logger.LogWarning($"No {nameof(IOpenModPlugin)} implementation found in assembly: {providerAssembly}; skipping loading of this assembly as plugin");
                providerAssemblies.Remove(providerAssembly);
            }

            //Lovely mono :(
            /*if (assembliesWithMissingDependencies.Count != 0)
            {
                m_Logger.LogWarning($"Some plugins can not load until server restart: {string.Join(", ", assembliesWithMissingDependencies)}", Color.DarkRed);
            }*/

            m_LoadedPluginAssemblies.AddRange(providerAssemblies.Select(d => new WeakReference(d)));
            return providerAssemblies;
        }

        private static readonly Regex MissingFileAssemblyVersionRegex = new Regex("'(?<assembly>.+?), Version=(?<version>.+?), ", RegexOptions.Compiled);//TypeLoad detect to entriers for this regex and one is wrong
        private static readonly Regex TypeLoadAssemblyVersionRegex = new Regex("assembly:(?<assembly>.+?), Version=(?<version>.+?), ", RegexOptions.Compiled);//Missing file dont have assembly:

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private Dictionary<string, SemVersion> CheckRequiredDependencies(IEnumerable<Exception> exLoaderExceptions)
        {
            var missingAssemblies = new Dictionary<string, SemVersion>(StringComparer.InvariantCulture);
            foreach (var typeEx in exLoaderExceptions)
            {
                Match match;
                switch (typeEx)
                {
                    case TypeLoadException _:
                        match = TypeLoadAssemblyVersionRegex.Match(typeEx.Message);
                        break;

                    case FileNotFoundException _:
                        match = MissingFileAssemblyVersionRegex.Match(typeEx.Message);
                        break;

                    default:
                        continue;
                }

                if (!match.Success)
                    continue;

                var assemblyName = match.Groups["assembly"].Value;
                var version = Version.Parse(match.Groups["version"].Value);

                //Example: Version is 1.1.2.0 to SemVersion is 1.1.0+2 but we need 1.1.2 to install from nuget
                var assemblyVersion = new SemVersion(version.Major, version.Minor, version.Build); 

                if (missingAssemblies.TryGetValue(assemblyName, out var versionToInstall) &&
                    versionToInstall > assemblyVersion)
                    continue;

                missingAssemblies[assemblyName] = assemblyVersion;
            }

            return missingAssemblies;
        }

        private async Task<bool> TryInstallRequiredDependenciesAsync(Assembly providerAssembly, Dictionary<string, SemVersion> missingAssemblies)
        {
            foreach (var assembly in missingAssemblies)
            {
                var packagetToInstall = await m_NuGetPackageManager.QueryPackageExactAsync(assembly.Key, assembly.Value.ToString());
                var result = await m_NuGetPackageManager.InstallAsync(packagetToInstall.Identity);
                if (result.Code == NuGetInstallCode.Success || result.Code == NuGetInstallCode.NoUpdatesFound)
                    continue;

                m_Logger.LogWarning($"Failed to install \"{assembly.Key}\": " + result.Code, Color.DarkRed);
                m_Logger.LogWarning($"Plugin {providerAssembly} can not load without it!", Color.DarkRed);
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            // Clear assembly references
            m_LoadedPluginAssemblies.Clear();
        }
    }
}