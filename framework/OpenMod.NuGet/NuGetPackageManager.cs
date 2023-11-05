using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using OpenMod.Common.Hotloading;
using OpenMod.NuGet.Helpers;
using OpenMod.Common.Helpers;
using NuGet.Versioning;

namespace OpenMod.NuGet
{
    public class NuGetPackageManager : IDisposable
    {
        public delegate Assembly AssemblyLoader(byte[] assemblyData, byte[]? assemblySymbols);

        public ILogger Logger { get; set; }
        public string PackagesDirectory { get; }

        private readonly ISettings m_NugetSettings;
        private readonly NuGetFramework m_CurrentFramework;
        private readonly FrameworkReducer m_FrameworkReducer;
        private readonly PackagePathResolver m_PackagePathResolver;
        private readonly PackageResolver m_PackageResolver;
        private readonly PackageSourceProvider m_PackageSourceProvider;
        private readonly SourceRepositoryProvider m_SourceRepositoryProvider;
        private readonly Dictionary<AssemblyName, Assembly> m_ResolveCache;
        private readonly Dictionary<string, List<NuGetAssembly>> m_LoadedPackageAssemblies;
        private readonly HashSet<string> m_IgnoredDependendencies;
        private readonly PackagesDataStore? m_PackagesDataStore;
        private readonly Dictionary<string, PackageIdentity> m_CachedPackageIdentity;

        private static readonly string[] s_PackageBlacklist =
        {
            // Trademark violations
            "VaultPlugin",
            "F.AntiCosmetics",
            "F.ItemRestrictions"
        };

        private static readonly string[] s_PublisherBlacklist = new string[] { };

        private static readonly ConcurrentDictionary<string, List<Assembly>> s_LoadedPackages = new();
        public bool AssemblyResolverInstalled => m_AssemblyResolverInstalled;
        private bool m_AssemblyResolverInstalled;
        private AssemblyLoader m_AssemblyLoader;

        public NuGetPackageManager(string packagesDirectory) : this(packagesDirectory, usePackagesFiles: true)
        {
        }

        public NuGetPackageManager(string packagesDirectory, bool usePackagesFiles)
        {
            if (string.IsNullOrEmpty(packagesDirectory))
            {
                throw new ArgumentException(nameof(packagesDirectory));
            }

            m_AssemblyLoader = Hotloader.LoadAssembly;

            PackagesDirectory = packagesDirectory;
            if (!Directory.Exists(packagesDirectory))
            {
                Directory.CreateDirectory(packagesDirectory);
            }

            if (usePackagesFiles)
            {
                m_PackagesDataStore = new PackagesDataStore(Path.Combine(packagesDirectory, "packages.yaml"));
                m_PackagesDataStore.EnsureExistsAsync().GetAwaiter().GetResult();
            }

            Logger = new NullLogger();

            const string nugetFile = "NuGet.Config";

            var nugetConfig = Path.Combine(packagesDirectory, nugetFile);
            if (!File.Exists(nugetConfig))
            {
                var nl = Environment.NewLine;

                File.WriteAllText(nugetConfig,
                    $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{nl}"
                    + $"<configuration>{nl}"
                    + $"    <packageSources>{nl}"
                    + $"        <add key=\"nuget.org\" value=\"https://api.nuget.org/v3/index.json\" protocolVersion=\"3\" />{nl}"
                    + $"    </packageSources>{nl}"
                    + "</configuration>");
            }

            m_NugetSettings = Settings.LoadDefaultSettings(packagesDirectory, nugetFile, null);

            m_FrameworkReducer = new FrameworkReducer();

#if NETSTANDARD2_1_OR_GREATER
            // checking if running under Mono
            if (RuntimeEnvironmentHelper.IsMono)
#else
            if (false)
#endif
            {
                // Using Mono that supports .netstandard2.1 and .net4.8.0
                var net48 = new NuGetFramework(".NETFramework", new Version(4, 8, 0, 0));
                var net481 = new NuGetFramework(".NETFramework", new Version(4, 8, 1, 0));
                m_CurrentFramework = new FallbackFramework(FrameworkConstants.CommonFrameworks.NetStandard21, new List<NuGetFramework>
                {
                    FrameworkConstants.CommonFrameworks.Net461,
                    FrameworkConstants.CommonFrameworks.Net462,
                    FrameworkConstants.CommonFrameworks.Net47,
                    FrameworkConstants.CommonFrameworks.Net471,
                    FrameworkConstants.CommonFrameworks.Net472,
                    net48,
                    net481
                });
            }
            else
            {
                var frameworkName = typeof(NuGetPackageManager).Assembly
                    .GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>()
                    ?.FrameworkName;

                m_CurrentFramework = frameworkName == null
                    ? NuGetFramework.AnyFramework
                    : NuGetFramework.Parse(frameworkName);
            }

            m_PackagePathResolver = new PackagePathResolver(packagesDirectory);
            m_PackageResolver = new PackageResolver();
            m_LoadedPackageAssemblies = new Dictionary<string, List<NuGetAssembly>>(StringComparer.OrdinalIgnoreCase);
            m_ResolveCache = new Dictionary<AssemblyName, Assembly>(AssemblyNameEqualityComparer.Instance);
            m_IgnoredDependendencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            m_CachedPackageIdentity = new(StringComparer.OrdinalIgnoreCase);

            m_PackageSourceProvider = new(m_NugetSettings);
            m_SourceRepositoryProvider = new(m_PackageSourceProvider, Repository.Provider.GetCoreV3());

            // ReSharper disable once VirtualMemberCallInConstructor
            InstallAssemblyResolver();
        }

        public virtual async Task<NuGetInstallResult> InstallAsync(PackageIdentity packageIdentity, bool allowPreReleaseVersions = false)
        {
            if (packageIdentity == null)
            {
                throw new ArgumentNullException(nameof(packageIdentity));
            }

            foreach (var id in s_PackageBlacklist)
            {
                if (packageIdentity.Id.Trim().Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    return new NuGetInstallResult(NuGetInstallCode.PackageOrVersionNotFound);
                }
            }

            using var cacheContext = new SourceCacheContext();
            NuGetQueryResult queryResult;
            try
            {
                queryResult = await QueryDependenciesAsync(packageIdentity, cacheContext, allowPreReleaseVersions);
            }
            catch (NuGetResolverInputException ex)
            {
                Logger.LogDebug(ex.ToString());
                return new NuGetInstallResult(NuGetInstallCode.PackageOrVersionNotFound);
            }

            if (queryResult.Code != NuGetInstallCode.Success)
            {
                return new NuGetInstallResult(queryResult.Code, queryResult.InstalledPackage);
            }

            if (queryResult.Packages?.Count > 0)
            {
                var globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(m_NugetSettings);

                foreach (var dependencyPackage in queryResult.Packages)
                {
                    if (m_IgnoredDependendencies.Contains(dependencyPackage.Id))
                    {
                        continue;
                    }

                    var installed = await GetLatestPackageIdentityAsync(dependencyPackage.Id);
                    if (installed != null && dependencyPackage.HasVersion && installed.Version >= dependencyPackage.Version)
                    {
                        continue;
                    }

                    var installedPath = GetNugetPackageFile(dependencyPackage);
                    if (!File.Exists(installedPath))
                    {
                        Logger.LogInformation(
                            $"Downloading: {dependencyPackage.Id} v{dependencyPackage.Version.OriginalVersion}");

                        var downloadResource =
                            await dependencyPackage.Source.GetResourceAsync<DownloadResource>(CancellationToken.None);

                        using var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                            dependencyPackage,
                            new PackageDownloadContext(cacheContext),
                            globalPackagesFolder,
                            Logger, CancellationToken.None);

                        downloadResult.PackageStream.CopyToFile(installedPath);

                        m_CachedPackageIdentity[dependencyPackage.Id] = downloadResult.PackageReader.GetIdentity();
                    }

                    await LoadAssembliesFromNuGetPackageAsync(installedPath);
                }
            }

            await RemoveOutdatedPackagesAsync();

            if (m_PackagesDataStore != null)
            {
                await m_PackagesDataStore.AddOrUpdatePackageIdentity(packageIdentity);
            }

            return new NuGetInstallResult(packageIdentity);
        }

        public async Task RemoveOutdatedPackagesAsync()
        {
            var installedVersions = new Dictionary<string, List<PackageIdentity>>(StringComparer.OrdinalIgnoreCase);

            foreach (var directory in Directory.GetDirectories(PackagesDirectory))
            {
                var dirName = new DirectoryInfo(directory).Name;
                var nupkgFile = Path.Combine(directory, dirName + ".nupkg");

                if (!File.Exists(nupkgFile))
                {
                    Logger.LogDebug("Package not found:" + nupkgFile);
                    continue;
                }

                using var packageReader = new PackageArchiveReader(nupkgFile);
                var identity = await packageReader.GetIdentityAsync(CancellationToken.None);

                if (!installedVersions.TryGetValue(identity.Id, out var versions))
                {
                    versions = new(1);
                    installedVersions.Add(identity.Id, versions);
                }

                versions.Add(identity);
            }

            foreach (var kvp in installedVersions)
            {
                var identities = kvp.Value
                    .OrderByDescending(d => d.Version)
                    .ToArray();

                if (identities.Length == 1)
                {
                    continue;
                }

                foreach (var identity in identities.Skip(1))
                {
                    await RemoveAsync(identity);
                }

                m_CachedPackageIdentity[kvp.Key] = identities[0];
            }
        }

        public virtual async Task<bool> IsPackageInstalledAsync(string packageId)
        {
            return await GetLatestPackageIdentityAsync(packageId) != null;
        }

        public virtual async Task<IPackageSearchMetadata?> QueryPackageExactAsync(string packageId, string? version = null, bool includePreRelease = false)
        {
            if (string.IsNullOrEmpty(packageId))
            {
                throw new ArgumentNullException(nameof(packageId));
            }

            var matches = await QueryPackagesAsync(packageId, version, includePreRelease);
            return matches.FirstOrDefault(d => d.Identity.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase));
        }

        public virtual async Task<IEnumerable<IPackageSearchMetadata>> QueryPackagesAsync(string packageId, string? version = null, bool includePreRelease = false)
        {
            if (string.IsNullOrEmpty(packageId))
            {
                throw new ArgumentNullException(nameof(packageId));
            }

            var matches = new List<IPackageSearchMetadata>();
            var searchFilter = new SearchFilter(includePreRelease)
            {
                IncludeDelisted = false,
                SupportedFrameworks = m_CurrentFramework is FallbackFramework ff
                    ? ff.Fallback.Select(x => x.GetShortFolderName())
                    : new[] { m_CurrentFramework.GetShortFolderName() }
            };
            var nugetVersion = version == null
                ? null
                : new NuGetVersion(version);

            Logger.LogInformation("Searching repository for package: " + packageId);

            foreach (var sourceRepository in m_SourceRepositoryProvider.GetRepositories())
            {
                var searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();

                IPackageSearchMetadata[] searchResult;
                try
                {
                    searchResult = (await searchResource.SearchAsync(packageId, searchFilter, 0, 10, Logger, CancellationToken.None)).ToArray();
                }
                catch (Exception ex)
                {
                    Logger.LogDebug("Could not find package: ");
                    Logger.LogDebug(ex.ToString());
                    continue;
                }

                if (nugetVersion == null)
                {
                    Logger.LogDebug("version == null, adding searchResult: " + searchResult.Length);
                    matches.AddRange(searchResult);
                    continue;
                }

                foreach (var packageMeta in searchResult)
                {
                    var versions = await packageMeta.GetVersionsAsync();
                    if (!versions.Any(d => d.Version.Equals(nugetVersion, VersionComparison.Default)))
                    {
                        continue;
                    }

                    Logger.LogDebug("adding packageMeta: "
                                    + packageMeta.Identity.Id
                                    + ":"
                                    + packageMeta.Identity.Version);
                    matches.Add(packageMeta);
                }
                break;
            }

            return matches.Where(a =>
            {
                if (s_PackageBlacklist.Any(p => p.Equals(a.Identity?.Id, StringComparison.OrdinalIgnoreCase)))
                    return false;

                if (s_PublisherBlacklist.Any(p => a.Owners.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;

                return true;
            });
        }

        public void IgnoreDependencies(params string[] packageIds)
        {
            if (packageIds == null)
            {
                throw new ArgumentNullException(nameof(packageIds));
            }

            m_IgnoredDependendencies.UnionWith(packageIds);
        }

        public virtual Task<ICollection<PackageDependency>> GetDependenciesAsync(PackageIdentity identity)
        {
            return GetDependenciesInternalAsync(identity, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        }

        private async Task<ICollection<PackageDependency>> GetDependenciesInternalAsync(PackageIdentity identity, ISet<string> lookedUpIds)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            if (!lookedUpIds.Add(identity.Id))
            {
                return Array.Empty<PackageDependency>();
            }

            var nupkgFile = GetNugetPackageFile(identity);
            if (!File.Exists(nupkgFile))
            {
                throw new Exception($"GetDependenciesAsync on a nupkg that doesn't exist: {identity.Id} v{identity.Version}");
            }

            using var packageReader = new PackageArchiveReader(nupkgFile);
            var dependencyGroups = (await packageReader.GetPackageDependenciesAsync(CancellationToken.None)).ToList();

            if (dependencyGroups.Count == 0)
            {
                return Array.Empty<PackageDependency>();
            }

            var framework = m_FrameworkReducer.GetNearest(m_CurrentFramework, dependencyGroups.Select(d => d.TargetFramework));
            if (framework == null)
            {
                throw new Exception($"Failed to get dependencies of {identity.Id} v{identity.Version}: no supported framework found. {Environment.NewLine} Requested framework: {m_CurrentFramework.DotNetFrameworkName}, available frameworks: {string.Join(", ", dependencyGroups.Select(d => d.TargetFramework).Select(d => d.DotNetFrameworkName))}");
            }

            var dependencies = new HashSet<PackageDependency>(dependencyGroups
                .Find(d => d.TargetFramework == framework)
                .Packages
                .Where(d => !m_IgnoredDependendencies.Contains(d.Id)), PackageDependencyComparerSlim.Default);

            foreach (var dependency in dependencies.ToList())
            {
                var dependencyPackage = await GetLatestPackageIdentityAsync(dependency.Id);
                if (dependencyPackage == null)
                {
                    throw new Exception($"Failed to get dependencies of {identity.Id} v{identity.Version}: dependency {dependency.Id} {dependency.VersionRange.OriginalString} is not installed!");
                }

                dependencies.UnionWith(await GetDependenciesInternalAsync(dependencyPackage, lookedUpIds));
            }

            return dependencies;
        }

        public void SetAssemblyLoader(AssemblyLoader assemblyLoader)
        {
            m_AssemblyLoader = assemblyLoader ?? throw new ArgumentNullException(nameof(assemblyLoader));
        }

        public virtual async Task<NuGetQueryResult> QueryDependenciesAsync(PackageIdentity identity, SourceCacheContext cacheContext, bool allowPreReleaseVersions)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            if (cacheContext == null)
            {
                throw new ArgumentNullException(nameof(cacheContext));
            }

            var sourceRepositories = m_SourceRepositoryProvider.GetRepositories();
            var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);

            if (!identity.HasVersion)
            {
                identity = await QueryPackageLatestVersionAsync(identity, cacheContext, sourceRepositories, allowPreReleaseVersions);
            }

            await QueryPackageDependenciesAsync(identity, cacheContext, sourceRepositories, availablePackages, allowPreReleaseVersions);

            if (availablePackages.Count == 0)
            {
                if (await GetLatestPackageIdentityAsync(identity.Id) != null)
                {
                    // latest version already installed
                    return new NuGetQueryResult(identity, NuGetInstallCode.NoUpdatesFound);
                }

                // package doesnt exist or weird NuGet bug:
                // package shows up in version list but can not be installed yet
                // will need to wait 15-30 minutes until it is indexed
                return new NuGetQueryResult(identity, NuGetInstallCode.PackageOrVersionNotFound);
            }

            var resolverContext = new PackageResolverContext(
                DependencyBehavior.Lowest,
                new[] { identity.Id },
                Enumerable.Empty<string>(),
                Enumerable.Empty<PackageReference>(),
                Enumerable.Empty<PackageIdentity>(),
                availablePackages,
                sourceRepositories.Select(s => s.PackageSource),
                Logger);

            var resolvedPackages = m_PackageResolver.Resolve(resolverContext, CancellationToken.None)
                .Select(p => availablePackages.Single(x
                    => PackageIdentityComparer.Default.Equals(x, p)))
                .Where(d => !m_IgnoredDependendencies.Contains(d.Id))
                .ToList();

            return new NuGetQueryResult(resolvedPackages);
        }

        public virtual Task<IEnumerable<Assembly>> LoadAssembliesFromNuGetPackageAsync(string nupkgFile)
        {
            return LoadAssembliesFromNuGetPackageInternalAsync(nupkgFile, true);
        }

        private async Task<IEnumerable<Assembly>> LoadAssembliesFromNuGetPackageInternalAsync(string nupkgFile, bool getAndLoadDependencies)
        {
            if (string.IsNullOrEmpty(nupkgFile))
            {
                throw new ArgumentNullException(nameof(nupkgFile));
            }

            var fullPath = Path.GetFullPath(nupkgFile);

            if (s_LoadedPackages.TryGetValue(fullPath, out var loadedPackages))
            {
                return loadedPackages;
            }

            if (m_LoadedPackageAssemblies.TryGetValue(fullPath, out var loadedAssemblies))
            {
                if (loadedAssemblies.All(d => d.Assembly.IsAlive))
                {
                    return loadedAssemblies.ConvertAll(x => (Assembly)x.Assembly.Target);
                }

                m_LoadedPackageAssemblies.Remove(fullPath);
            }

            var packageName = Path.GetFileName(nupkgFile);

            Logger.LogInformation("Loading NuGet package: " + packageName);

            using var packageReader = new PackageArchiveReader(nupkgFile);
            var identity = await packageReader.GetIdentityAsync(CancellationToken.None);

            if (getAndLoadDependencies)
            {
                foreach (var dependency in await GetDependenciesAsync(identity))
                {
                    if (dependency.Id.Equals(identity.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var package = await GetLatestPackageIdentityAsync(dependency.Id);
                    if (package == null)
                    {
                        throw new Exception($"Failed to load assemblies from {nupkgFile}: dependency {dependency.Id} {dependency.VersionRange.OriginalString} is not installed.");
                    }

                    var nupkg = GetNugetPackageFile(package);
                    await LoadAssembliesFromNuGetPackageInternalAsync(nupkg, false);
                }
            }

            var assemblies = new List<NuGetAssembly>();

            var libItems = (await packageReader.GetLibItemsAsync(CancellationToken.None)).ToList();
            var nearest = m_FrameworkReducer.GetNearest(m_CurrentFramework, libItems.Select(x => x.TargetFramework));
            var file = libItems.Find(x => x.TargetFramework == nearest);

            foreach (var item in file?.Items ?? Enumerable.Empty<string>())
            {
                try
                {
                    if (!item.EndsWith(".dll", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var assemblyData = packageReader.ReadAllBytes(item);
                    var assemblySymbolsPath = Path.ChangeExtension(item, "pdb");
                    var assemblySymbols = file!.Items.Contains(assemblySymbolsPath)
                        ? packageReader.ReadAllBytes(assemblySymbolsPath)
                        : null;

                    var asm = m_AssemblyLoader(assemblyData, assemblySymbols);
                    var assemblyName = Hotloader.GetRealAssemblyName(asm);

                    assemblies.Add(new NuGetAssembly
                    {
                        Assembly = new WeakReference(asm),
                        AssemblyName2 = assemblyName,
                        Package = identity
                    });
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to load assembly at {item} from file {nupkgFile}");
                    Logger.LogError(ex.ToString());
                }
            }

            m_LoadedPackageAssemblies.Add(fullPath, assemblies);

            var result = assemblies.ConvertAll(d => (Assembly)d.Assembly.Target);
            s_LoadedPackages.TryAdd(fullPath, result);
            return result;
        }

        public virtual async Task<PackageIdentity?> GetLatestPackageIdentityAsync(string packageId)
        {
            if (string.IsNullOrEmpty(packageId))
            {
                throw new ArgumentNullException(nameof(packageId));
            }

            if (m_CachedPackageIdentity.TryGetValue(packageId, out var package))
            {
                return package;
            }

            var packageIdentities = new List<PackageIdentity>();
            foreach (var dir in Directory.GetDirectories(PackagesDirectory))
            {
                var directoryName = new DirectoryInfo(dir).Name;
                if (!directoryName.StartsWith(packageId + ".", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var nupkgFile = Path.Combine(dir, directoryName + ".nupkg");
                if (!File.Exists(nupkgFile))
                {
                    Debug.Fail("File should exists");
                    return null;
                }

                using var reader = new PackageArchiveReader(nupkgFile);
                var identity = await reader.GetIdentityAsync(CancellationToken.None);
                if (identity.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase))
                {
                    packageIdentities.Add(identity);
                }
            }

            package = packageIdentities.OrderByDescending(c => c.Version).FirstOrDefault();
            if (package is not null)
            {
                m_CachedPackageIdentity[packageId] = package;
            }

            return package;
        }

        public virtual string GetNugetPackageFile(PackageIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            var dirName = m_PackagePathResolver.GetPackageDirectoryName(identity);

            return Path.Combine(PackagesDirectory, dirName, dirName + ".nupkg");
        }

        protected virtual async Task<PackageIdentity> QueryPackageLatestVersionAsync(
            PackageIdentity package,
            SourceCacheContext cacheContext,
            IEnumerable<SourceRepository> repositories,
            bool allowPreReleaseVersions)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            if (cacheContext == null)
            {
                throw new ArgumentNullException(nameof(cacheContext));
            }

            if (repositories == null)
            {
                throw new ArgumentNullException(nameof(repositories));
            }

            Logger.LogDebug("GetPackageLatestVersion: " + package);

            foreach (var sourceRepository in repositories)
            {
                Logger.LogDebug("GetResourceAsync (MetadataResource) for " + sourceRepository.PackageSource.SourceUri);
                var metadataResource = await sourceRepository.GetResourceAsync<MetadataResource>();

                Logger.LogDebug("GetLatestVersion");
                var latestVersion = await metadataResource.GetLatestVersion(package.Id, allowPreReleaseVersions, false, cacheContext, Logger, CancellationToken.None);
                if (latestVersion == null || (package.HasVersion && package.Version < latestVersion))
                {
                    continue;
                }

                package = new PackageIdentity(package.Id, latestVersion);
            }

            return package;
        }

        protected virtual async Task QueryPackageDependenciesAsync(
            PackageIdentity package,
            SourceCacheContext cacheContext,
            IEnumerable<SourceRepository> repositories,
            ISet<SourcePackageDependencyInfo> availablePackages,
            bool allowPreReleaseVersions)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            if (cacheContext == null)
            {
                throw new ArgumentNullException(nameof(cacheContext));
            }

            if (repositories == null)
            {
                throw new ArgumentNullException(nameof(repositories));
            }

            if (availablePackages == null)
            {
                throw new ArgumentNullException(nameof(availablePackages));
            }

            if (availablePackages.Contains(package))
            {
                return;
            }

            foreach (var sourceRepository in repositories)
            {
                Logger.LogDebug("GetPackageDependencies: " + package);
                var dependencyInfo = await FindDependencyInfoAsync(package, cacheContext, sourceRepository);

                if (dependencyInfo == null)
                {
                    continue;
                }

                availablePackages.Add(dependencyInfo);
                if (!dependencyInfo.Dependencies.Any())
                {
                    return;
                }

                Logger.LogDebug("GetResourceAsync (FindPackageById) for " + dependencyInfo.Source.PackageSource.SourceUri);

                var dependenciesInfo = dependencyInfo.Dependencies.ToList();
                var bag = new ConcurrentBag<PackageDependency>(dependenciesInfo);
                var deps = new ConcurrentBag<PackageIdentity>();

                await FindBestDependencies(cacheContext, sourceRepository, bag, deps, allowPreReleaseVersions);

                if (dependenciesInfo.Count != deps.Count)
                {
                    Logger.LogDebug("Some packages are not found, trying to find them in other sources");

                    var missingDependencies = dependenciesInfo.ToList();
                    UpdateMissingDependenciesList(missingDependencies, deps);

                    foreach (var sourceRepository2 in repositories.Where(r => r != sourceRepository))
                    {
                        Logger.LogDebug($"Searching {missingDependencies.Count} missing packages in {sourceRepository2.PackageSource.SourceUri}");

                        var copiedMissingDependencies = new ConcurrentBag<PackageDependency>(missingDependencies);
                        await FindBestDependencies(cacheContext, sourceRepository2, copiedMissingDependencies, deps, allowPreReleaseVersions);

                        UpdateMissingDependenciesList(missingDependencies, deps);

                        if (missingDependencies.Count == 0)
                        {
                            break;
                        }
                    }
                }

                foreach (var dependency in deps)
                {
                    await QueryPackageDependenciesAsync(dependency, cacheContext, repositories, availablePackages, allowPreReleaseVersions);
                }

                return;
            }

            void UpdateMissingDependenciesList(List<PackageDependency> missingDependencies, ConcurrentBag<PackageIdentity> allDependencies)
            {
                for (var i = missingDependencies.Count - 1; i >= 0; i--)
                {
                    var dependency = missingDependencies[i];
                    if (allDependencies.Any(d => d.Id.Equals(dependency.Id, StringComparison.OrdinalIgnoreCase)))
                    {
                        missingDependencies.RemoveAt(i);
                        continue;
                    }
                }
            }
        }

        private async Task FindBestDependencies(SourceCacheContext cacheContext, SourceRepository sourceRepository,
            ConcurrentBag<PackageDependency> dependencies, ConcurrentBag<PackageIdentity> result, bool allowPreReleaseVersions)
        {
            var resource = await sourceRepository.GetResourceAsync<FindPackageByIdResource>();

            const int maxConcurrencyTasks = 5;

            // Task.WhenAll( dependencyInfo.Dependencies.Select ) starts all tasks, but which for some reason causes slower loading
            var tasks = Enumerable.Range(0, Math.Min(maxConcurrencyTasks, dependencies.Count))
                .Select(async _ =>
                {
                    while (dependencies.TryTake(out var dependency))
                    {
                        var latestInstalledPackage = await GetLatestPackageIdentityAsync(dependency.Id);
                        if (latestInstalledPackage != null
                            && dependency.VersionRange.IsBetter(null, latestInstalledPackage.Version))
                        {
                            result.Add(latestInstalledPackage);
                            continue;
                        }

                        var versions = await resource.GetAllVersionsAsync(dependency.Id, cacheContext, Logger, CancellationToken.None);
                        if (!versions.Any())
                        {
                            continue;
                        }

                        versions = allowPreReleaseVersions
                            ? versions
                            : versions.Where(x => !x.IsPrerelease);

                        result.Add(new PackageIdentity(dependency.Id, dependency.VersionRange.FindBestMatch(versions)));
                    }
                });

            await Task.WhenAll(tasks);
        }

        private async Task<SourcePackageDependencyInfo?> FindDependencyInfoAsync(
            PackageIdentity package,
            SourceCacheContext cacheContext,
            SourceRepository sourceRepository)
        {
            Logger.LogDebug("GetResourceAsync (DependencyInfoResource) for " + sourceRepository.PackageSource.SourceUri);
            var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();

            Logger.LogDebug("ResolvePackage");

            SourcePackageDependencyInfo? dependencyInfo;
            dependencyInfo = await dependencyInfoResource.ResolvePackage(package, m_CurrentFramework, cacheContext, Logger, CancellationToken.None);

            if (dependencyInfo == null)
            {
                Logger.LogDebug("Dependency was not found: " + package + " in " + sourceRepository.PackageSource.SourceUri);
                return null;
            }

            Logger.LogDebug("Dependency was found: " + package + " in " + sourceRepository.PackageSource.SourceUri);

            return dependencyInfo;
        }

        public virtual void InstallAssemblyResolver()
        {
            if (m_AssemblyResolverInstalled)
            {
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            m_AssemblyResolverInstalled = true;
        }

        private Assembly? OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            if (m_ResolveCache.TryGetValue(name, out var assembly))
            {
                return assembly;
            }

            var matchedAssembly = m_LoadedPackageAssemblies.Values
                .SelectMany(d => d)
                .Where(d => d.Assembly.IsAlive && AssemblyNameEqualityComparer.Instance.Equals(d.AssemblyName2, name))
                .OrderByDescending(d => d.AssemblyName2.Version)
                .FirstOrDefault();

            assembly = (matchedAssembly?.Assembly.Target as Assembly) ?? Hotloader.FindAssembly(name);

            assembly ??= s_LoadedPackages.Values
                .SelectMany(x => x)
                .Select(x => (assembly: x, name: x.GetName()))
                .Where(x => AssemblyNameEqualityComparer.Instance.Equals(x.name, name))
                .OrderByDescending(x => x.name.Version)
                .FirstOrDefault()
                .assembly;

            if (assembly != null)
            {
                m_ResolveCache.Add(name, assembly);
            }

            Logger.LogDebug(assembly == null
                ? $"Failed to resolve {args.Name}"
                : $"Resolved assembly from NuGet: {args.Name} @ v{assembly.GetName().Version}");

            return assembly;
        }

        public async Task InstallPackagesAsync(ICollection<PackageIdentity> packages, bool updateExisting = true)
        {
            foreach (var package in packages)
            {
                var installedPackage = await GetLatestPackageIdentityAsync(package.Id);
                if (installedPackage == null)
                {
                    await LogAndInstallPackage(package, false);
                    continue;
                }

                if (!updateExisting)
                {
                    continue;
                }

                if (!package.HasVersion)
                {
                    await LogAndInstallPackage(package, true);
                    continue;
                }

                if (installedPackage?.Version == null || package.Version > installedPackage.Version)
                {
                    await LogAndInstallPackage(package, false);
                }
            }
        }

        private async Task LogAndInstallPackage(PackageIdentity package, bool isUpdate)
        {
            Logger.LogInformation($"{(isUpdate ? "Updating" : "Installing")} package: {package.Id}@{package.Version?.OriginalVersion ?? "latest"}");
            await InstallAsync(package, allowPreReleaseVersions: true);
        }

        public async Task<int> InstallMissingPackagesAsync(bool updateExisting = true)
        {
            if (m_PackagesDataStore == null)
            {
                throw new Exception("InstallMissingPackagesAsync failed: packages.yaml is not enabled.");
            }

            var packages = await m_PackagesDataStore.GetPackagesAsync();

            await InstallPackagesAsync(packages, updateExisting);

            return packages.Count;
        }

        public async Task<bool> RemoveAsync(PackageIdentity packageIdentity)
        {
            if (packageIdentity == null)
            {
                throw new ArgumentNullException(nameof(packageIdentity));
            }

            var installDir = m_PackagePathResolver.GetInstallPath(packageIdentity);
            if (installDir == null || !Directory.Exists(installDir))
            {
                return false;
            }

            try
            {
                Directory.Delete(installDir, recursive: true);
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Failed to delete directory \"{installDir}\" " + ex.Message);
                return false;
            }

            if (m_PackagesDataStore != null)
            {
                await m_PackagesDataStore.RemovePackageAsync(packageIdentity.Id);
            }

            m_CachedPackageIdentity.Remove(packageIdentity.Id);

            return true;
        }

        public void Dispose()
        {
            if (m_AssemblyResolverInstalled)
            {
                AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
                m_AssemblyResolverInstalled = false;
            }

            ClearCache();
        }

        public virtual ICollection<NuGetAssembly> GetLoadedAssemblies()
        {
            return m_LoadedPackageAssemblies.Values
                .SelectMany(d => d)
                .ToList();
        }

        public void ClearCache(bool clearGlobalCache = false)
        {
            foreach (var kv in m_LoadedPackageAssemblies)
            {
                kv.Value?.Clear();
            }
            m_LoadedPackageAssemblies.Clear();

            m_ResolveCache.Clear();
            m_CachedPackageIdentity.Clear();

            if (clearGlobalCache)
            {
                s_LoadedPackages.Clear();
            }
        }
    }
}
