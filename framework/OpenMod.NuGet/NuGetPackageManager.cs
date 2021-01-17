using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
using OpenMod.Common.Hotloading;

namespace OpenMod.NuGet
{
    public class NuGetPackageManager : IDisposable
    {
        private ILogger m_Logger;
        public ILogger Logger
        {
            get { return m_Logger; }
            set { m_Logger = value ?? new NullLogger(); }
        }

        public string PackagesDirectory { get; }
        private readonly List<Lazy<INuGetResourceProvider>> m_Providers;
        private readonly ISettings m_NugetSettings;
        private readonly NuGetFramework m_CurrentFramework;
        private readonly FrameworkReducer m_FrameworkReducer;
        private readonly PackagePathResolver m_PackagePathResolver;
        private readonly PackageResolver m_PackageResolver;
        private readonly Dictionary<string, Assembly> m_ResolveCache;
        private readonly Dictionary<string, List<NuGetAssembly>> m_LoadedPackageAssemblies;
        private readonly HashSet<string> m_IgnoredDependendencies;
        private readonly PackagesDataStore m_PackagesDataStore;

        private static readonly Dictionary<string, List<Assembly>> s_LoadedPackages = new Dictionary<string, List<Assembly>>();
        private static readonly Regex s_VersionRegex = new Regex("Version=(?<version>.+?), ", RegexOptions.Compiled);
        private bool m_AssemblyResolverInstalled;
        private Func<byte[], Assembly> m_AssemblyLoader;

        public NuGetPackageManager(string packagesDirectory) : this(packagesDirectory, usePackagesFiles: true)
        {

        }

        public NuGetPackageManager(string packagesDirectory, bool usePackagesFiles)
        {
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
            m_Providers = new List<Lazy<INuGetResourceProvider>>();
            m_Providers.AddRange(Repository.Provider.GetCoreV3()); // Add v3 API support

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
            m_LoadedPackageAssemblies = new Dictionary<string, List<NuGetAssembly>>();
            m_ResolveCache = new Dictionary<string, Assembly>();
            m_IgnoredDependendencies = new HashSet<string>();
            InstallAssemblyResolver();
        }

        public virtual async Task<NuGetInstallResult> InstallAsync(PackageIdentity packageIdentity, bool allowPrereleaseVersions = false)
        {
            using var cacheContext = new SourceCacheContext();
            IEnumerable<SourcePackageDependencyInfo> dependencyPackages;
            try
            {
                dependencyPackages = await QueryDependenciesAsync(packageIdentity, cacheContext);
            }
            catch (NuGetResolverInputException ex)
            {
                Logger.LogDebug(ex.ToString());
                return new NuGetInstallResult(NuGetInstallCode.PackageOrVersionNotFound);
            }

            var packageExtractionContext = new PackageExtractionContext(
                PackageSaveMode.Nupkg,
                XmlDocFileSaveMode.None,
                ClientPolicyContext.GetClientPolicy(m_NugetSettings, Logger),
                Logger);

            foreach (var dependencyPackage in dependencyPackages)
            {
                if (await GetLatestPackageIdentityAsync(dependencyPackage.Id) != null)
                {
                    continue;
                }

                var installedPath = m_PackagePathResolver.GetInstalledPath(dependencyPackage);
                if (installedPath == null)
                {
                    Logger.LogInformation($"Downloading: {dependencyPackage.Id} v{dependencyPackage.Version.OriginalVersion}");

                    var downloadResource = await dependencyPackage.Source.GetResourceAsync<DownloadResource>(CancellationToken.None);
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
                }

                await LoadAssembliesFromNuGetPackageAsync(GetNugetPackageFile(dependencyPackage));
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
                    m_Logger.LogDebug("Package not found:" + nupkgFile);
                    continue;
                }

                using var packageReader = new PackageArchiveReader(nupkgFile);
                var identity = await packageReader.GetIdentityAsync(CancellationToken.None);

                if (!installedVersions.ContainsKey(identity.Id))
                {
                    installedVersions.Add(identity.Id, new List<NuGetVersion>());
                }

                installedVersions[identity.Id].Add(identity.Version);
            }

            foreach (var package in installedVersions.Keys)
            {
                var versions = installedVersions[package]
                    .OrderByDescending(d => d);

                foreach (var version in versions.Skip(1))
                {
                    await RemoveAsync(new PackageIdentity(package, version));
                }
            }
        }

        public virtual async Task<bool> IsPackageInstalledAsync(string packageId)
        {
            return (await GetLatestPackageIdentityAsync(packageId)) != null;
        }

        public virtual async Task<IPackageSearchMetadata> QueryPackageExactAsync(string packageId, string version = null, bool includePreRelease = false)
        {
            var matches = await QueryPackagesAsync(packageId, version, includePreRelease);
            return matches.FirstOrDefault(d => d.Identity.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase));
        }

        public virtual async Task<IEnumerable<IPackageSearchMetadata>> QueryPackagesAsync(string packageId, string version = null, bool includePreRelease = false)
        {
            var matches = new List<IPackageSearchMetadata>();

            Logger.LogInformation("Searching repository for package: " + packageId);

            var packageSourceProvider = new PackageSourceProvider(m_NugetSettings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, m_Providers);

            var sourceRepositories = sourceRepositoryProvider.GetRepositories();

            foreach (var sourceRepository in sourceRepositories)
            {
                var searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();
                var searchFilter = new SearchFilter(includePreRelease)
                {
                    IncludeDelisted = false,
                    SupportedFrameworks = new[] { m_CurrentFramework.DotNetFrameworkName }
                };

                IEnumerable<IPackageSearchMetadata> searchResult;
                try
                {
                    searchResult = (await searchResource.SearchAsync(packageId, searchFilter, 0, 10, Logger, CancellationToken.None)).ToList();
                }
                catch (Exception ex)
                {
                    Logger.LogDebug("Could not find package: ");
                    Logger.LogDebug(ex.ToString());
                    continue;
                }

                if (version == null)
                {
                    Logger.LogDebug("version == null, adding searchResult: " + searchResult.Count());
                    matches.AddRange(searchResult);
                    continue;
                }

                foreach (var packageMeta in searchResult)
                {
                    var versions = (await packageMeta.GetVersionsAsync()).ToList();
                    if (!versions.Any(d
                        => d.Version.OriginalVersion.Equals(version, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    Logger.LogDebug("adding packageMeta: "
                                    + packageMeta.Identity.Id
                                    + ":"
                                    + packageMeta.Identity.Version);
                    matches.Add(packageMeta);
                }
            }

            return matches;
        }

        public void IgnoreDependencies(params string[] packageIds)
        {
            m_IgnoredDependendencies.AddRange(packageIds);
        }

        public virtual async Task<ICollection<PackageDependency>> GetDependenciesAsync(PackageIdentity identity)
        {
            return await GetDependenciesInternalAsync(identity, new List<string>());
        }

        private async Task<ICollection<PackageDependency>> GetDependenciesInternalAsync(PackageIdentity identity, List<string> lookedUpIds)
        {

            if (lookedUpIds.Any(d => string.Equals(d, identity.Id, StringComparison.OrdinalIgnoreCase)))
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

            var framework = m_FrameworkReducer.GetNearest(m_CurrentFramework, dependencyGroups.Select(d => d.TargetFramework));
            if (framework == null)
            {
                throw new Exception($"Failed to get dependencies of {identity.Id} v{identity.Version}: no supported framework found. {Environment.NewLine} Requested framework: {m_CurrentFramework.DotNetFrameworkName}, available frameworks: {string.Join(", ", dependencyGroups.Select(d => d.TargetFramework).Select(d => d.DotNetFrameworkName))}");
            }

            var packages = dependencyGroups
                .First(d => d.TargetFramework == framework)
                .Packages;

            list.AddRange(packages);
            list.RemoveAll(d => m_IgnoredDependendencies.Contains(d.Id));

            foreach (var dependency in list.ToList())
            {
                if (m_IgnoredDependendencies.Contains(dependency.Id))
                {
                    continue;
                }

                var dependencyPackage = await GetLatestPackageIdentityAsync(dependency.Id);
                if (dependencyPackage == null)
                {
                    throw new Exception($"Failed to get dependencies of {identity.Id} v{identity.Version}: dependency {dependency.Id} {dependency.VersionRange.OriginalString} is not installed!");
                }

                list.AddRange(await GetDependenciesInternalAsync(dependencyPackage, lookedUpIds));
            }

            return list;
        }

        public void SetAssemblyLoader(Func<byte[], Assembly> assemblyLoader)
        {
            m_AssemblyLoader = assemblyLoader;
        }

        public virtual async Task<ICollection<SourcePackageDependencyInfo>> QueryDependenciesAsync(PackageIdentity identity, SourceCacheContext cacheContext)
        {
            var packageSourceProvider = new PackageSourceProvider(m_NugetSettings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, m_Providers);

            var sourceRepositories = sourceRepositoryProvider.GetRepositories();

            var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
            await QueryPackageDependenciesAsync(identity, cacheContext, sourceRepositories, availablePackages);

            var resolverContext = new PackageResolverContext(
                DependencyBehavior.Lowest,
                new[] { identity.Id },
                Enumerable.Empty<string>(),
                Enumerable.Empty<PackageReference>(),
                Enumerable.Empty<PackageIdentity>(),
                availablePackages,
                sourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
                Logger);

            return m_PackageResolver.Resolve(resolverContext, CancellationToken.None)
                .Select(p => availablePackages.Single(x
                    => PackageIdentityComparer.Default.Equals(x, p)))
                .Where(d => !m_IgnoredDependendencies.Contains(d.Id))
                .ToList();
        }

        public virtual async Task<List<byte[]>> LoadAssembliesFromNuGetPackageRawAsync(string nupkgFile)
        {
            using var packageReader = new PackageArchiveReader(nupkgFile);

            var libItems = packageReader.GetLibItems().ToList();
            var nearest = m_FrameworkReducer.GetNearest(m_CurrentFramework, libItems.Select(x => x.TargetFramework));
            var assemblies = new List<byte[]>();

            foreach (var file in libItems.Where(x => x.TargetFramework.Equals(nearest)))
            {
                foreach (var item in file.Items)
                {
                    try
                    {
                        if (!item.EndsWith(".dll"))
                        {
                            continue;
                        }

                        var entry = packageReader.GetEntry(item);
                        using var stream = entry.Open();
                        var ms = new MemoryStream();
                        await stream.CopyToAsync(ms);

                        try
                        {
                            assemblies.Add(ms.ToArray());
                        }
                        finally
                        {
                            ms.Close();
                            stream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Failed to load assembly at {item} from file {nupkgFile}");
                        Logger.LogError(ex.ToString());
                    }
                }
            }

            return assemblies;
        }

        public virtual async Task<IEnumerable<Assembly>> LoadAssembliesFromNuGetPackageAsync(string nupkgFile)
        {
            if (s_LoadedPackages.ContainsKey(nupkgFile))
            {
                return s_LoadedPackages[nupkgFile];
            }

            using var packageReader = new PackageArchiveReader(nupkgFile);
            var identity = await packageReader.GetIdentityAsync(CancellationToken.None);
            var versionedIdentity = identity.Id;
            if (identity.HasVersion)
            {
                versionedIdentity += $"-{identity.Version.OriginalVersion}";
            }

            if (s_LoadedPackages.ContainsKey(versionedIdentity))
            {
                return s_LoadedPackages[versionedIdentity];
            }

            Logger.LogInformation("Loading NuGet package: " + Path.GetFileName(nupkgFile));

            var fullPath = Path.GetFullPath(nupkgFile).ToLower();

            if (m_LoadedPackageAssemblies.ContainsKey(fullPath))
            {
                if (m_LoadedPackageAssemblies[fullPath].All(d => d.Assembly.IsAlive))
                {
                    return m_LoadedPackageAssemblies[fullPath].Select(d => d.Assembly.Target).Cast<Assembly>();
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
                if (!File.Exists(nupkg))
                {
                    var latestInstalledVersion = await GetLatestPackageIdentityAsync(dependency.Id);
                    if (latestInstalledVersion == null)
                    {
                        Logger.LogWarning("Failed to resolve: " + dependency.Id);
                        continue;
                    }

                    nupkg = GetNugetPackageFile(latestInstalledVersion);
                }

                await LoadAssembliesFromNuGetPackageAsync(nupkg);
            }

            var libItems = packageReader.GetLibItems().ToList();
            var nearest = m_FrameworkReducer.GetNearest(m_CurrentFramework, libItems.Select(x => x.TargetFramework));

            foreach (var file in libItems.Where(x => x.TargetFramework.Equals(nearest)))
            {
                foreach (var item in file.Items)
                {
                    try
                    {
                        if (!item.EndsWith(".dll"))
                        {
                            continue;
                        }

                        var entry = packageReader.GetEntry(item);
                        using var stream = entry.Open();
                        var ms = new MemoryStream();
                        await stream.CopyToAsync(ms);

                        try
                        {
                            var asm = m_AssemblyLoader(ms.ToArray());

                            var name = GetVersionIndependentName(asm.FullName, out var extractedVersion);
                            var parsedVersion = new Version(extractedVersion);

                            assemblies.Add(new NuGetAssembly
                            {
                                Assembly = new WeakReference(asm),
                                AssemblyName = name,
                                Version = parsedVersion,
                                Package = identity
                            });
                        }
                        finally
                        {
                            ms.Close();
                            stream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Failed to load assembly at {item} from file {nupkgFile}");
                        Logger.LogError(ex.ToString());
                    }
                }
            }

            m_LoadedPackageAssemblies.Add(fullPath, assemblies);
            var result = assemblies.Select(d => d.Assembly.Target).Cast<Assembly>().ToList();

            s_LoadedPackages.Add(versionedIdentity, result);
            s_LoadedPackages.Add(nupkgFile, result);
            return result;
        }

        public virtual async Task<PackageIdentity> GetLatestPackageIdentityAsync(string packageId)
        {
            if (!Directory.Exists(PackagesDirectory))
            {
                return null;
            }

            var packageIdentities = new List<PackageIdentity>();
            foreach (var dir in Directory.GetDirectories(PackagesDirectory))
            {
                var dirName = new DirectoryInfo(dir).Name;
                if (!dirName.StartsWith(packageId + ".", StringComparison.OrdinalIgnoreCase))
                    continue;

                var directoryName = new DirectoryInfo(dir).Name;
                var nupkgFile = Path.Combine(dir, directoryName + ".nupkg");
                if (!File.Exists(nupkgFile))
                {
                    return null;
                }

                using var reader = new PackageArchiveReader(nupkgFile);
                var identity = await reader.GetIdentityAsync(CancellationToken.None);

                if (identity.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase))
                {
                    packageIdentities.Add(identity);
                }
            }

            return packageIdentities.OrderByDescending(c => c.Version).FirstOrDefault();
        }

        public virtual string GetNugetPackageFile(PackageIdentity identity)
        {
            var dir = m_PackagePathResolver.GetInstallPath(identity);
            var dirName = new DirectoryInfo(dir).Name;

            return Path.Combine(dir, dirName + ".nupkg");
        }

        protected virtual async Task QueryPackageDependenciesAsync(PackageIdentity package,
                                                            SourceCacheContext cacheContext,
                                                            IEnumerable<SourceRepository> repositories,
                                                            ISet<SourcePackageDependencyInfo> availablePackages)
        {
            if (availablePackages.Contains(package))
            {
                return;
            }

            Logger.LogDebug("GetPackageDependencies: " + package);
            var repos = repositories.ToList();
            Logger.LogDebug($"Repos for {package}: " + repos.Count);

            foreach (var sourceRepository in repos)
            {
                Logger.LogDebug("GetResourceAsync for " + sourceRepository.PackageSource.SourceUri);
                var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();

                Logger.LogDebug("ResolvePackage");
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(package, m_CurrentFramework, cacheContext, Logger, CancellationToken.None);

                if (dependencyInfo == null)
                {
                    Logger.LogDebug("Dependency was not found: " + package + " in " + sourceRepository.PackageSource.SourceUri);
                    continue;
                }
                Logger.LogDebug("Dependency was found: " + package + " in " + sourceRepository.PackageSource.SourceUri);

                availablePackages.Add(dependencyInfo);
                foreach (var dependency in dependencyInfo.Dependencies)
                {
                    if (m_IgnoredDependendencies.Contains(dependency.Id))
                    {
                        continue;
                    }

                    await QueryPackageDependenciesAsync(new PackageIdentity(dependency.Id, dependency.VersionRange.MaxVersion ?? dependency.VersionRange.MinVersion), cacheContext, repos, availablePackages);
                }
            }
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

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = GetVersionIndependentName(args.Name, out _);
            if (m_ResolveCache.ContainsKey(name))
            {
                return m_ResolveCache[name];
            }

            var matchingAssemblies =
                m_LoadedPackageAssemblies.Values.SelectMany(d => d)
                    .Where(d => d.Assembly.IsAlive && d.AssemblyName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(d => d.Version);

            var result = (Assembly)matchingAssemblies.FirstOrDefault()?.Assembly.Target;
            if (result != null)
            {
                m_ResolveCache.Add(name, result);
            }

            m_Logger.LogDebug(result == null
                ? $"Failed to resolve {args.Name}"
                : $"Resolved assembly from NuGet: {args.Name} @ v{result.GetName().Version}");

            return result;
        }

        protected static string GetVersionIndependentName(string fullAssemblyName, out string extractedVersion)
        {
            var match = s_VersionRegex.Match(fullAssemblyName);
            extractedVersion = match.Groups[1].Value;
            return s_VersionRegex.Replace(fullAssemblyName, string.Empty);
        }

        public async Task InstallMissingPackagesAsync(bool updateExisting = true)
        {
            if (m_PackagesDataStore == null)
            {
                throw new Exception("InstallMissingPackagesAsync failed: packages.yaml is not enabled.");
            }

            var packages = await m_PackagesDataStore.GetPackagesAsync();
            foreach (var package in packages)
            {
                if (await IsPackageInstalledAsync(package.Id))
                {
                    if (updateExisting)
                    {
                        var installedPackage = await GetLatestPackageIdentityAsync(package.Id);
                        if (package.HasVersion && installedPackage.Version != package.Version)
                        {
                            await InstallAsync(package, allowPrereleaseVersions: true);
                        }
                    }

                    continue;
                }

                Logger.LogInformation($"Installing package: {package.Id}@{package.Version?.OriginalVersion ?? "latest"}");
                await InstallAsync(package, allowPrereleaseVersions: true);
            }
        }

        public async Task<bool> RemoveAsync(PackageIdentity packageIdentity)
        {
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
                m_Logger.LogWarning($"Failed to delete directory \"{installDir}\" " + ex.Message);
                return false;
            }

            if (m_PackagesDataStore != null)
            {
                await m_PackagesDataStore.AddOrUpdatePackageIdentity(packageIdentity);
            }

            return true;
        }

        public void Dispose()
        {
            if (m_AssemblyResolverInstalled)
            {
                AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
                m_AssemblyResolverInstalled = false;
            }

            foreach (var kv in m_LoadedPackageAssemblies)
            {
                kv.Value?.Clear();
            }

            m_LoadedPackageAssemblies.Clear();
        }

        public virtual ICollection<NuGetAssembly> GetLoadedAssemblies()
        {
            return m_LoadedPackageAssemblies.Values
                .SelectMany(d => d)
                .ToList();
        }

        public void ClearCache()
        {
            m_LoadedPackageAssemblies.Clear();
            m_ResolveCache.Clear();
        }
    }
}
