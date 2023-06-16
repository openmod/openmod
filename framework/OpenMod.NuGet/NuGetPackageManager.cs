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
using NuGet.Packaging.Signing;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using OpenMod.Common.Helpers;
using OpenMod.Common.Hotloading;
using OpenMod.NuGet.Helpers;

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
        private readonly Dictionary<string, Assembly> m_ResolveCache;
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

            var frameworkName = Assembly.GetExecutingAssembly().GetCustomAttributes(true)
                                           .OfType<System.Runtime.Versioning.TargetFrameworkAttribute>()
                                           .Select(x => x.FrameworkName)
                                           .FirstOrDefault();

            m_CurrentFramework = frameworkName == null
                ? NuGetFramework.AnyFramework
                : NuGetFramework.ParseFrameworkName(frameworkName, new DefaultFrameworkNameProvider());

            m_PackagePathResolver = new PackagePathResolver(packagesDirectory);
            m_PackageResolver = new PackageResolver();
            m_LoadedPackageAssemblies = new Dictionary<string, List<NuGetAssembly>>(StringComparer.OrdinalIgnoreCase);
            m_ResolveCache = new Dictionary<string, Assembly>();
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
                var packageExtractionContext = new PackageExtractionContext(
                PackageSaveMode.Nupkg,
                XmlDocFileSaveMode.None,
                ClientPolicyContext.GetClientPolicy(m_NugetSettings, Logger),
                Logger);

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

                    var installedPath = m_PackagePathResolver.GetInstalledPath(dependencyPackage);
                    if (installedPath == null)
                    {
                        Logger.LogInformation(
                            $"Downloading: {dependencyPackage.Id} v{dependencyPackage.Version.OriginalVersion}");

                        var downloadResource =
                            await dependencyPackage.Source.GetResourceAsync<DownloadResource>(CancellationToken.None);

                        var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                            dependencyPackage,
                            new PackageDownloadContext(cacheContext),
                            SettingsUtility.GetGlobalPackagesFolder(m_NugetSettings),
                            Logger, CancellationToken.None);

                        await PackageExtractor.ExtractPackageAsync(
                             downloadResult.PackageSource,
                             downloadResult.PackageStream,
                             m_PackagePathResolver,
                             packageExtractionContext,
                             CancellationToken.None);

                        m_CachedPackageIdentity.Remove(dependencyPackage.Id);
                    }

                    await LoadAssembliesFromNuGetPackageAsync(GetNugetPackageFile(dependencyPackage));
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
            var installedVersions = new Dictionary<string, List<NuGetVersion>>();

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
                    versions = new();
                    installedVersions.Add(identity.Id, versions);
                }

                versions.Add(identity.Version);
            }

            foreach (var kvp in installedVersions)
            {
                var versions = kvp.Value
                    .OrderByDescending(d => d);

                foreach (var version in versions.Skip(1))
                {
                    await RemoveAsync(new PackageIdentity(kvp.Key, version));
                }
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
                SupportedFrameworks = new[] { m_CurrentFramework.DotNetFrameworkName } // todo: add support for from .net461 to .net481
            };

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

                if (version == null)
                {
                    Logger.LogDebug("version == null, adding searchResult: " + searchResult.Length);
                    matches.AddRange(searchResult);
                    continue;
                }

                foreach (var packageMeta in searchResult)
                {
                    var versions = await packageMeta.GetVersionsAsync();
                    if (!versions.Any(d => d.Version.OriginalVersion.Equals(version, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    Logger.LogDebug("adding packageMeta: "
                                    + packageMeta.Identity.Id
                                    + ":"
                                    + packageMeta.Identity.Version);
                    matches.Add(packageMeta);
                }
            }

            return matches.Where(a =>
            {
                return !s_PackageBlacklist.Any(l => string.Equals(a.Identity?.Id?.Trim(), l, StringComparison.OrdinalIgnoreCase))
                    && !s_PublisherBlacklist.Any(l => a.Owners?.ToLowerInvariant().Contains(l.ToLowerInvariant()) ?? false);
            });
        }

        public void IgnoreDependencies(params string[] packageIds)
        {
            if (packageIds == null)
            {
                throw new ArgumentNullException(nameof(packageIds));
            }

            m_IgnoredDependendencies.AddRange(packageIds);
        }

        public virtual Task<ICollection<PackageDependency>> GetDependenciesAsync(PackageIdentity identity)
        {
            return GetDependenciesInternalAsync(identity, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        }

        private async Task<ICollection<PackageDependency>> GetDependenciesInternalAsync(PackageIdentity identity, ICollection<string> lookedUpIds)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            if (lookedUpIds.Contains(identity.Id))
            {
                return new List<PackageDependency>();
            }

            lookedUpIds.Add(identity.Id);
            var nupkgFile = GetNugetPackageFile(identity);

            if (!File.Exists(nupkgFile))
            {
                throw new Exception($"GetDependenciesAsync on a nupkg that doesn't exist: {identity.Id} v{identity.Version}");
            }

            using var packageReader = new PackageArchiveReader(nupkgFile);
            var list = new List<PackageDependency>();
            var dependencyGroups = (await packageReader.GetPackageDependenciesAsync(CancellationToken.None)).ToList();

            if (dependencyGroups.Count == 0)
            {
                return list;
            }

            var framework = m_FrameworkReducer.GetNearest(m_CurrentFramework,
                dependencyGroups.Select(d => d.TargetFramework));
            if (framework == null)
            {
                throw new Exception($"Failed to get dependencies of {identity.Id} v{identity.Version}: no supported framework found. {Environment.NewLine} Requested framework: {m_CurrentFramework.DotNetFrameworkName}, available frameworks: {string.Join(", ", dependencyGroups.Select(d => d.TargetFramework).Select(d => d.DotNetFrameworkName))}");
            }

            var packages = dependencyGroups
                .Find(d => d.TargetFramework == framework)
                .Packages;

            list.AddRange(packages);
            list.RemoveAll(d => m_IgnoredDependendencies.Contains(d.Id));

            foreach (var dependency in list.ToList())
            {
                var dependencyPackage = await GetLatestPackageIdentityAsync(dependency.Id);
                if (dependencyPackage == null)
                {
                    throw new Exception($"Failed to get dependencies of {identity.Id} v{identity.Version}: dependency {dependency.Id} {dependency.VersionRange.OriginalString} is not installed!");
                }

                list.AddRange(await GetDependenciesInternalAsync(dependencyPackage, lookedUpIds));
            }

            return list;
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

        public virtual async Task<IEnumerable<Assembly>> LoadAssembliesFromNuGetPackageAsync(string nupkgFile)
        {
            if (string.IsNullOrEmpty(nupkgFile))
            {
                throw new ArgumentNullException(nameof(nupkgFile));
            }

            if (s_LoadedPackages.TryGetValue(nupkgFile, out var loadedPackages))
            {
                return loadedPackages;
            }

            using var packageReader = new PackageArchiveReader(nupkgFile);
            var identity = await packageReader.GetIdentityAsync(CancellationToken.None);
            var versionedIdentity = identity.Id;
            if (identity.HasVersion)
            {
                versionedIdentity += $"-{identity.Version.OriginalVersion}";
            }

            if (s_LoadedPackages.TryGetValue(versionedIdentity, out loadedPackages))
            {
                return loadedPackages;
            }

            Logger.LogInformation("Loading NuGet package: " + Path.GetFileName(nupkgFile));

            var fullPath = Path.GetFullPath(nupkgFile);

            if (m_LoadedPackageAssemblies.TryGetValue(fullPath, out var loadedAssemblies))
            {
                if (loadedAssemblies.All(d => d.Assembly.IsAlive))
                {
                    return loadedAssemblies.ConvertAll(x => (Assembly)x.Assembly.Target);
                }

                m_LoadedPackageAssemblies.Remove(fullPath);
            }

            var assemblies = new List<NuGetAssembly>();

            var dependencies = await GetDependenciesAsync(identity);
            foreach (var dependency in dependencies.Where(d => !d.Id.Equals(identity.Id)))
            {
                var package = await GetLatestPackageIdentityAsync(dependency.Id);
                if (package == null)
                {
                    throw new Exception($"Failed to load assemblies from {nupkgFile}: dependency {dependency.Id} {dependency.VersionRange.OriginalString} is not installed.");
                }

                var nupkg = GetNugetPackageFile(package);
                await LoadAssembliesFromNuGetPackageAsync(nupkg);
            }

            var libItems = (await packageReader.GetLibItemsAsync(CancellationToken.None)).ToList();
            var nearest = m_FrameworkReducer.GetNearest(m_CurrentFramework, libItems.Select(x => x.TargetFramework));
            var file = libItems.Find(x => x.TargetFramework == nearest);

            if (file != null)
            {
                foreach (var item in file.Items)
                {
                    try
                    {
                        if (!item.EndsWith(".dll", StringComparison.Ordinal))
                        {
                            continue;
                        }

                        var assemblyData = packageReader.ReadAllBytes(item);
                        var assemblySymbolsPath = Path.ChangeExtension(item, "pdb");
                        var assemblySymbols = file.Items.Contains(assemblySymbolsPath)
                            ? packageReader.ReadAllBytes(assemblySymbolsPath)
                            : null;

                        var asm = m_AssemblyLoader(assemblyData, assemblySymbols);

                        var name = ReflectionExtensions.GetVersionIndependentName(asm.FullName, out var extractedVersion);
                        var parsedVersion = new Version(extractedVersion);

                        assemblies.Add(new NuGetAssembly
                        {
                            Assembly = new WeakReference(asm),
                            AssemblyName = name,
                            Version = parsedVersion,
                            Package = identity
                        });
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Failed to load assembly at {item} from file {nupkgFile}");
                        Logger.LogError(ex.ToString());
                    }
                }
            }

            m_LoadedPackageAssemblies.Add(fullPath, assemblies);
            var result = assemblies.ConvertAll(d => (Assembly)d.Assembly.Target);

            s_LoadedPackages.TryAdd(versionedIdentity, result);
            s_LoadedPackages.TryAdd(nupkgFile, result);
            return result;
        }

        public virtual async Task<PackageIdentity?> GetLatestPackageIdentityAsync(string packageId)
        {
            if (string.IsNullOrEmpty(packageId))
            {
                throw new ArgumentNullException(nameof(packageId));
            }

            if (m_CachedPackageIdentity.TryGetValue(packageId, out var result))
            {
                return result;
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

            var package = packageIdentities.OrderByDescending(c => c.Version).FirstOrDefault();
            if (package is not null && !m_CachedPackageIdentity.ContainsKey(packageId))
            {
                m_CachedPackageIdentity.Add(packageId, package);
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

            Logger.LogDebug("GetPackageDependencies: " + package);
            var dependencyInfo = await FindDependencyInfoAsync(package, cacheContext, repositories);

            if (dependencyInfo == null)
            {
                return;
            }

            availablePackages.Add(dependencyInfo);

            foreach (var sourceRepository in repositories)
            {
                Logger.LogDebug("GetResourceAsync (FindPackageById) for " + dependencyInfo.Source.PackageSource.SourceUri);
                var packageResource = await sourceRepository.GetResourceAsync<FindPackageByIdResource>();

                foreach (var dependency in dependencyInfo.Dependencies)
                {
                    var versions = (await packageResource.GetAllVersionsAsync(dependency.Id, cacheContext, Logger, CancellationToken.None))
                        .ToArray();

                    if (versions.Length == 0)
                    {
                        Logger.LogDebug("Versions could not be found: " + package + " in " + sourceRepository.PackageSource.SourceUri);
                        continue;
                    }

                    await QueryPackageDependenciesAsync(new PackageIdentity(dependency.Id, dependency.VersionRange.FindBestMatch(versions)), cacheContext, repositories, availablePackages, allowPreReleaseVersions);
                }
            }
        }

        private async Task<SourcePackageDependencyInfo?> FindDependencyInfoAsync(
            PackageIdentity package,
            SourceCacheContext cacheContext,
            IEnumerable<SourceRepository> repositories)
        {
            foreach (var sourceRepository in repositories)
            {
                Logger.LogDebug("GetResourceAsync (DependencyInfoResource) for " + sourceRepository.PackageSource.SourceUri);
                var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();

                Logger.LogDebug("ResolvePackage");
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(package, m_CurrentFramework, cacheContext, Logger, CancellationToken.None);
                if (dependencyInfo == null)
                {
                    Logger.LogDebug("Dependency was not found: " + package + " in " + sourceRepository.PackageSource.SourceUri);
                    continue;
                }

                Logger.LogDebug("Dependency was found: " + package + " in " + sourceRepository.PackageSource.SourceUri);

                return dependencyInfo;
            }

            return null;
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
            var name = ReflectionExtensions.GetVersionIndependentName(args.Name);
            if (m_ResolveCache.TryGetValue(name, out var assembly))
            {
                return assembly;
            }

            var matchingAssemblies =
                m_LoadedPackageAssemblies.Values.SelectMany(d => d)
                    .Where(d => d.Assembly.IsAlive && (d.AssemblyName.Equals(name, StringComparison.OrdinalIgnoreCase)
                        || Hotloader.GetRealAssemblyName((Assembly)d.Assembly.Target).Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(d => d.Version);

            assembly = matchingAssemblies.FirstOrDefault()?.Assembly.Target as Assembly ?? Hotloader.GetAssembly(args.Name);

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
                if (!await IsPackageInstalledAsync(package.Id))
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

                var installedPackage = await GetLatestPackageIdentityAsync(package.Id);
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

        public void ClearCache()
        {
            foreach (var kv in m_LoadedPackageAssemblies)
            {
                kv.Value?.Clear();
            }
            m_LoadedPackageAssemblies.Clear();

            m_ResolveCache.Clear();
            m_CachedPackageIdentity.Clear();
        }
    }
}
