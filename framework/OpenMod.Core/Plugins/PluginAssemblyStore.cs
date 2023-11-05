﻿using Microsoft.Extensions.Logging;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using OpenMod.API;
using OpenMod.API.Plugins;
using OpenMod.NuGet;
using Semver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMod.Core.Plugins
{
    [OpenModInternal]
    public class PluginAssemblyStore : IPluginAssemblyStore, IDisposable
    {
        private readonly ILogger<PluginAssemblyStore> m_Logger;
        private readonly NuGetPackageManager m_NuGetPackageManager;

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
        }

        private readonly List<WeakReference> m_LoadedPluginAssemblies = new();
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

        private async Task<ICollection<Assembly>> InstallPackagesInEmbeddedFile(Assembly assembly,
            ICollection<Assembly> ignoredAssemblies)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var newPlugins = new List<Assembly>();

            try
            {
                var packagesResourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(x => x.EndsWith("packages.yaml"));
                if (packagesResourceName == null)
                    return newPlugins;

                var packagesRosourceInfo = assembly.GetManifestResourceInfo(packagesResourceName);
                m_Logger.LogDebug("Found packages.yaml with name {PackagesResourceName}.", packagesResourceName);
                m_Logger.LogDebug("Package resource info {PackagesRosourceInfo}.", packagesRosourceInfo);

#if NETSTANDARD2_1_OR_GREATER
                await using var stream = assembly.GetManifestResourceStream(packagesResourceName);
#else
                using var stream = assembly.GetManifestResourceStream(packagesResourceName);
#endif
                if (stream == null) return newPlugins;

                using var reader = new StreamReader(stream);
                var packagesContent = await reader.ReadToEndAsync();

                var deserialized = deserializer.Deserialize<SerializedPackagesFile>(packagesContent).Packages;

                var packages = deserialized?.Select(d => new PackageIdentity(d.Id,
                        d.Version.Equals("latest", StringComparison.OrdinalIgnoreCase)
                            ? null
                            : new NuGetVersion(d.Version)))
                    .ToList();

                if (packages == null || packages.Count == 0) return newPlugins;

                m_Logger.LogInformation(
                    "Found and installing embedded NuGet packages for plugin assembly: {AssemblyName}",
                    assembly.GetName().Name);

                var existingPackages = m_NuGetPackageManager.GetLoadedAssemblies();

                await m_NuGetPackageManager.InstallPackagesAsync(packages);

                var newAssemblies = m_NuGetPackageManager.GetLoadedAssemblies().Except(existingPackages);

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var newAssembly in newAssemblies.Select(x => (Assembly)x.Assembly.Target))
                {
                    if (newAssembly == null) continue;

                    if (newAssembly.GetCustomAttribute<PluginMetadataAttribute>() == null) continue;

                    if (ignoredAssemblies.Contains(newAssembly)) continue;

                    if (m_LoadedPluginAssemblies.Select(x => x.Target).Cast<Assembly>().Contains(newAssembly)) continue;

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

                    var missingAssemblies = CheckRequiredDependencies(ex.LoaderExceptions);
                    if (!TryInstallMissingDependencies)
                    {
                        m_Logger.LogWarning(
                            "Couldn't load plugin from {ProviderAssembly}: Failed to resolve required dependencies: {MissingAssemblies}",
                            providerAssembly, string.Join(", ", missingAssemblies.Keys));
                        continue;
                    }

                    //todo check result and try reload lib
                    await TryInstallRequiredDependenciesAsync(providerAssembly, missingAssemblies);
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

            m_LoadedPluginAssemblies.AddRange(providerAssemblies.Select(d => new WeakReference(d)));
            return providerAssemblies;
        }

        private static readonly Regex s_MissingFileAssemblyVersionRegex =
            new("'(?<assembly>.+?), Version=(?<version>.+?), ",
                RegexOptions.Compiled); //TypeLoad detect to entriers for this regex and one is wrong

        private static readonly Regex s_TypeLoadAssemblyVersionRegex =
            new("assembly:(?<assembly>.+?), Version=(?<version>.+?), ",
                RegexOptions.Compiled); //Missing file dont have assembly:

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private Dictionary<string, SemVersion> CheckRequiredDependencies(IEnumerable<Exception> exLoaderExceptions)
        {
            var missingAssemblies = new Dictionary<string, SemVersion>(StringComparer.Ordinal);
            foreach (var typeEx in exLoaderExceptions)
            {
                Match match;
                switch (typeEx)
                {
                    case TypeLoadException:
                        match = s_TypeLoadAssemblyVersionRegex.Match(typeEx.Message);
                        break;

                    case FileNotFoundException:
                        match = s_MissingFileAssemblyVersionRegex.Match(typeEx.Message);
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
                    SemVersion.ComparePrecedence(versionToInstall, assemblyVersion) > 0)
                    continue;

                missingAssemblies[assemblyName] = assemblyVersion;
            }

            return missingAssemblies;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private async Task<bool> TryInstallRequiredDependenciesAsync(Assembly providerAssembly,
            Dictionary<string, SemVersion> missingAssemblies)
        {
            foreach (var assembly in missingAssemblies)
            {
                var packagetToInstall =
                    await m_NuGetPackageManager.QueryPackageExactAsync(assembly.Key, assembly.Value.ToString());
                if (packagetToInstall == null)
                {
                    m_Logger.LogWarning(
                        "Package not found: {AssemblyName}. Plugin \"{ProviderAssemblyName}\" can't load without it!",
                        assembly.Key, providerAssembly.GetName().Name);
                    return false;
                }

                var result = await m_NuGetPackageManager.InstallAsync(packagetToInstall.Identity);
                // ReSharper disable once InvertIf
                if (result.Code is not (NuGetInstallCode.Success or NuGetInstallCode.NoUpdatesFound))
                {
                    m_Logger.LogWarning(
                        "Failed to install \"{AssemblyName}\": {ResultCode}. Plugin \"{ProviderAssemblyName}\" can't load without it!",
                        assembly.Key, result.Code, providerAssembly.GetName().Name);
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