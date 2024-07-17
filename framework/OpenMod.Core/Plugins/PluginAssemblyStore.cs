using Microsoft.Extensions.Logging;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using OpenMod.API;
using OpenMod.API.Plugins;
using OpenMod.NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OpenMod.Common.Helpers;
using OpenMod.Common.Hotloading;
using OpenMod.API.Persistence;
using VYaml.Serialization;
using OpenMod.NuGet.Persistence;

namespace OpenMod.Core.Plugins
{
    [OpenModInternal]
    public class PluginAssemblyStore : IPluginAssemblyStore, IDisposable
    {
        private readonly ILogger<PluginAssemblyStore> m_Logger;
        private readonly NuGetPackageManager m_NuGetPackageManager;
        private readonly YamlSerializerOptions m_YamlOptions;

        /// <summary>
        /// Defines if OpenMod would try to install missing dependencies.
        /// </summary>
        public static bool TryInstallMissingDependencies { get; set; }

        public PluginAssemblyStore(
            ILogger<PluginAssemblyStore> logger,
            NuGetPackageManager nuGetPackageManager)
        {
            m_Logger = logger;
            m_NuGetPackageManager = nuGetPackageManager;

            m_YamlOptions = new YamlSerializerOptions
            {
                Resolver = CompositeResolver.Create([YamlHashSetTypeFormatter<SerializedNuGetPackage>.Instance], StandardResolver.DefaultResolvers)
            };
        }

        private readonly List<WeakReference<Assembly>> m_LoadedPluginAssemblies = [];
        public IReadOnlyCollection<Assembly> LoadedPluginAssemblies
        {
            get
            {
                return m_LoadedPluginAssemblies
                    .Select(d => d.TryGetTarget(out var refer) ? refer : null)
                    .OfType<Assembly>()
                    .ToList();
            }
        }

        private async Task<ICollection<Assembly>> InstallPackagesInEmbeddedFile(Assembly assembly,
            ICollection<Assembly> ignoredAssemblies)
        {
            var newPlugins = new List<Assembly>();

            try
            {
                var packagesResourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(x => x.EndsWith("packages.yaml"));
                if (packagesResourceName == null)
                {
                    return newPlugins;
                }

                var packagesRosourceInfo = assembly.GetManifestResourceInfo(packagesResourceName);
                m_Logger.LogDebug("Found packages.yaml with name {PackagesResourceName}.", packagesResourceName);
                m_Logger.LogDebug("Package resource info {PackagesRosourceInfo}.", packagesRosourceInfo);

                await using var stream = assembly.GetManifestResourceStream(packagesResourceName);
                if (stream == null)
                {
                    return newPlugins;
                }

                var packagesData = stream.ReadAllBytes();
                var deserialized = YamlSerializer.Deserialize<SerializedPackagesFile>(packagesData, m_YamlOptions);
                var packages = deserialized?.Packages.Select(d => new PackageIdentity(d.Id,
                        d.Version.Equals("latest", StringComparison.OrdinalIgnoreCase)
                            ? null
                            : new NuGetVersion(d.Version)))
                    .ToList();

                if (packages == null || packages.Count == 0)
                {
                    return newPlugins;
                }

                m_Logger.LogInformation(
                    "Found and installing embedded NuGet packages for plugin assembly: {AssemblyName}",
                    assembly.GetName().Name);

                var existingPackages = m_NuGetPackageManager.GetLoadedAssemblies();

                await m_NuGetPackageManager.InstallPackagesAsync(packages);

                var newAssemblies = m_NuGetPackageManager.GetLoadedAssemblies().Except(existingPackages);

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var newAssembly in newAssemblies.Select(x => (Assembly)x.Assembly.Target))
                {
                    if (newAssembly == null)
                    {
                        continue;
                    }

                    if (newAssembly.GetCustomAttribute<PluginMetadataAttribute>() == null)
                    {
                        continue;
                    }

                    if (ignoredAssemblies.Contains(newAssembly))
                    {
                        continue;
                    }

                    if (m_LoadedPluginAssemblies.Any(x => x.TryGetTarget(out var refe) && refe == newAssembly))
                    {
                        continue;
                    }

                    newPlugins.Add(newAssembly);
                }
            }
            catch (Exception ex)
            {
                newPlugins.Clear();
                m_Logger.LogError(ex, "Failed to check/load embedded NuGet packages for assembly: {Assembly}",
                    assembly);
            }

            return newPlugins;
        }

        public async Task<ICollection<Assembly>> LoadPluginAssembliesAsync(IPluginAssembliesSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var providerAssemblies = await source.LoadPluginAssembliesAsync();

            foreach (var providerAssembly in providerAssemblies.ToList())
            {
                var pluginMetadata = providerAssembly.GetCustomAttribute<PluginMetadataAttribute>();
                if (pluginMetadata == null)
                {
                    m_Logger.LogWarning(
                        "No plugin metadata attribute found in assembly: {ProviderAssembly}; skipping loading of this assembly as plugin",
                        providerAssembly);
                    providerAssemblies.Remove(providerAssembly);
                    continue;
                }

                providerAssemblies.AddRange(await InstallPackagesInEmbeddedFile(providerAssembly, providerAssemblies));
            }

            foreach (var providerAssembly in providerAssemblies.ToList())
            {
                ICollection<Type> types;
                try
                {
                    types = providerAssembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    //Remove Assembly from loading because required dependencies are missing.
                    providerAssemblies.Remove(providerAssembly);

                    foreach (var loaderException in ex.LoaderExceptions)
                    {
                        m_Logger.LogDebug(loaderException,
                            "Exception occurred when getting types in plugin assembly: {ProviderAssemblyName}",
                            providerAssembly.GetName().Name);
                    }

                    var missingAssemblies = ex.GetMissingDependencies().ToList();
                    m_Logger.LogWarning(
                        "Couldn't load plugin from {ProviderAssembly}: Failed to resolve required dependencies: {MissingAssemblies}",
                        providerAssembly, string.Join(", ", missingAssemblies));

                    if (!TryInstallMissingDependencies)
                    {
                        continue;
                    }

                    m_Logger.LogWarning("Trying to install missing dependencies");

                    var _ = await TryInstallRequiredDependenciesAsync(providerAssembly, missingAssemblies);
                    if (!Hotloader.Enabled)
                    {
                        continue;
                    }

                    //todo check result and try reload lib
                    //todo this implementation we will need a new API Interface with a collection of assemblies names
                    //also it must have a method to try load an assembly individually
                    //behaviour
                    //fail to install dependencies -> ignore
                    //success installing dependencies -> ask store our assembly source to load assembly again(hotloader needs to be on).
                    continue;
                }

                if (types.Any(d => d.GetInterfaces().Any(x => x == typeof(IOpenModPlugin)) && d is { IsAbstract: false, IsClass: true }))
                {
                    continue;
                }

                m_Logger.LogWarning(
                    "No {PluginInterfaceName} implementation found in assembly: {ProviderAssembly}; skipping loading of this assembly as plugin",
                    nameof(IOpenModPlugin), providerAssembly);
                providerAssemblies.Remove(providerAssembly);
            }

            m_LoadedPluginAssemblies.AddRange(providerAssemblies.Select(d => new WeakReference<Assembly>(d)));
            return providerAssemblies;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private async Task<bool> TryInstallRequiredDependenciesAsync(Assembly providerAssembly,
            IEnumerable<AssemblyName> missingAssemblies)
        {
            foreach (var assembly in missingAssemblies)
            {
                var packagetToInstall = await m_NuGetPackageManager.QueryPackageExactAsync(assembly.Name, assembly.Version.ToString(3));
                if (packagetToInstall == null)
                {
                    m_Logger.LogWarning(
                        "Package not found: {AssemblyName}. Plugin \"{ProviderAssemblyName}\" can't load without it!",
                        assembly.Name, providerAssembly.GetName().Name);
                    return false;
                }

                var result = await m_NuGetPackageManager.InstallAsync(packagetToInstall.Identity);
                // ReSharper disable once InvertIf
                if (result.Code is not (NuGetInstallCode.Success or NuGetInstallCode.NoUpdatesFound))
                {
                    m_Logger.LogWarning(
                        "Failed to install \"{AssemblyName}\": {ResultCode}. Plugin \"{ProviderAssemblyName}\" can't load without it!",
                        assembly.Name, result.Code, providerAssembly.GetName().Name);
                    return false;
                }
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