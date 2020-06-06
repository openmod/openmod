using System;
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
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;

namespace OpenMod.NuGet
{
    public class NuGetPackageManager
    {
        private readonly ILogger m_Logger;
        private readonly string m_PackagesDirectory;
        private readonly List<Lazy<INuGetResourceProvider>> m_Providers;
        private readonly ISettings m_NugetSettings;
        private readonly NuGetFramework m_CurrentFramework;
        private readonly FrameworkReducer m_FrameworkReducer;
        private readonly PackagePathResolver m_PackagePathResolver;
        private readonly PackageResolver m_PackageResolver;
        private readonly Dictionary<string, List<CachedNuGetAssembly>> m_LoadedPackageAssemblies;

        public NuGetPackageManager(ILogger logger, string packagesDirectory)
        {
            m_Logger = logger;
            m_PackagesDirectory = packagesDirectory;
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
                    + $"</configuration>");
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
            m_LoadedPackageAssemblies = new Dictionary<string, List<CachedNuGetAssembly>>();
            InstallAssemblyResolver();
        }

        public virtual async Task<NuGetInstallResult> InstallAsync(PackageIdentity packageIdentity, bool allowPrereleaseVersions = false)
        {
            using (var cacheContext = new SourceCacheContext())
            {
                IEnumerable<SourcePackageDependencyInfo> packagesToInstall;
                try
                {
                    packagesToInstall = await GetDependenciesAsync(packageIdentity, cacheContext);
                }
                catch (NuGetResolverInputException ex)
                {
                    m_Logger.LogDebug(ex.ToString());
                    return new NuGetInstallResult(NuGetInstallCode.PackageOrVersionNotFound);
                }

                var packageExtractionContext = new PackageExtractionContext(
                    PackageSaveMode.Nupkg,
                    XmlDocFileSaveMode.None,
                    ClientPolicyContext.GetClientPolicy(m_NugetSettings, m_Logger),
                    m_Logger);

                foreach (var packageToInstall in packagesToInstall)
                {
                    var installedPath = m_PackagePathResolver.GetInstalledPath(packageToInstall);
                    if (installedPath == null)
                    {
                        var downloadResource = await packageToInstall.Source.GetResourceAsync<DownloadResource>(CancellationToken.None);
                        var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                            packageToInstall,
                            new PackageDownloadContext(cacheContext),
                            SettingsUtility.GetGlobalPackagesFolder(m_NugetSettings),
                            m_Logger, CancellationToken.None);

                        await PackageExtractor.ExtractPackageAsync(
                            downloadResult.PackageSource,
                            downloadResult.PackageStream,
                            m_PackagePathResolver,
                            packageExtractionContext,
                            CancellationToken.None);
                    }
                }

                return new NuGetInstallResult(packageIdentity);
            }
        }

        public virtual async Task<bool> PackageExistsAsync(string packageId)
        {
            return (await GetLatestPackageIdentityAsync(packageId)) != null;
        }

        public virtual async Task<IPackageSearchMetadata> QueryPackageExactAsync(string packageId, string version = null, bool includePreRelease = false)
        {
            var matches = await QueryPackagesAsync(packageId, version, includePreRelease);
            return matches.FirstOrDefault(d => d.Identity.Id.Equals(packageId));
        }

        public virtual async Task<IEnumerable<IPackageSearchMetadata>> QueryPackagesAsync(string packageId, string version = null, bool includePreRelease = false)
        {
            var matches = new List<IPackageSearchMetadata>();

            m_Logger.LogInformation("Searching repository for package: " + packageId);

            PackageSourceProvider packageSourceProvider = new PackageSourceProvider(m_NugetSettings);
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
                    searchResult = (await searchResource.SearchAsync(packageId, searchFilter, 0, 10, m_Logger, CancellationToken.None)).ToList();
                }
                catch (Exception ex)
                {
                    m_Logger.LogDebug("Could not find package: ");
                    m_Logger.LogDebug(ex.ToString());
                    continue;
                }

                if (version == null)
                {
                    m_Logger.LogDebug("version == null, adding searchResult: " + searchResult.Count());
                    matches.AddRange(searchResult);
                    continue;
                }

                foreach (var packageMeta in searchResult)
                {
                    var versions = (await packageMeta.GetVersionsAsync()).ToList();
                    if (versions.Any(d
                        => d.Version.OriginalVersion.Equals(version, StringComparison.OrdinalIgnoreCase)))
                    {
                        m_Logger.LogDebug("adding packageMeta: "
                            + packageMeta.Identity.Id
                            + ":"
                            + packageMeta.Identity.Version);
                        matches.Add(packageMeta);
                    }
                }
            }

            return matches;
        }

        public virtual async Task<IEnumerable<SourcePackageDependencyInfo>> GetDependenciesAsync(PackageIdentity identity, SourceCacheContext cacheContext)
        {
            PackageSourceProvider packageSourceProvider = new PackageSourceProvider(m_NugetSettings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, m_Providers);

            var sourceRepositories = sourceRepositoryProvider.GetRepositories();

            var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
            await GetPackageDependenciesAsync(identity, cacheContext, sourceRepositories, availablePackages);

            var resolverContext = new PackageResolverContext(
                DependencyBehavior.Lowest,
                new[] { identity.Id },
                Enumerable.Empty<string>(),
                Enumerable.Empty<PackageReference>(),
                Enumerable.Empty<PackageIdentity>(),
                availablePackages,
                sourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
                m_Logger);

            return m_PackageResolver.Resolve(resolverContext, CancellationToken.None)
                                  .Select(p => availablePackages.Single(x
                                      => PackageIdentityComparer.Default.Equals(x, p)));
        }

        public virtual async Task<IEnumerable<Assembly>> LoadAssembliesFromNugetPackageAsync(string nupkgFile)
        {
            var fullPath = Path.GetFullPath(nupkgFile).ToLower();

            if (m_LoadedPackageAssemblies.ContainsKey(fullPath))
            {
                return m_LoadedPackageAssemblies[fullPath].Select(d => d.Assembly);
            }

            List<CachedNuGetAssembly> assemblies = new List<CachedNuGetAssembly>();

            var packageReader = new PackageArchiveReader(nupkgFile);
            var identity = await packageReader.GetIdentityAsync(CancellationToken.None);

            using (var cache = new SourceCacheContext())
            {
                var dependencies = await GetDependenciesAsync(identity, cache);
                foreach (var dependency in dependencies.Where(d => !d.Id.Equals(identity.Id)))
                {
                    var nupkg = GetNugetPackageFile(dependency);
                    if (!File.Exists(nupkg))
                    {
                        var latestInstalledVersion = await GetLatestPackageIdentityAsync(dependency.Id);
                        if (latestInstalledVersion == null)
                        {
                            m_Logger.LogWarning("Failed to resolve: " + dependency.Id);
                            continue;
                        }

                        nupkg = GetNugetPackageFile(latestInstalledVersion);
                    }

                    await LoadAssembliesFromNugetPackageAsync(nupkg);
                }
            }

            var libItems = packageReader.GetLibItems().ToList();
            var nearest = m_FrameworkReducer.GetNearest(m_CurrentFramework, libItems.Select(x => x.TargetFramework));

            foreach (var file in libItems.Where(x => x.TargetFramework.Equals(nearest)))
            {
                foreach (var item in file.Items)
                {
                    if (!item.EndsWith(".dll"))
                    {
                        continue;
                    }

                    var entry = packageReader.GetEntry(item);
                    using (var stream = entry.Open())
                    {
                        MemoryStream ms = new MemoryStream();
                        await stream.CopyToAsync(ms);

                        try
                        {
                            var asm = Assembly.Load(ms.ToArray());

                            var name = GetVersionIndependentName(asm.FullName, out string extractedVersion);
                            var parsedVersion = new Version(extractedVersion);

                            assemblies.Add(new CachedNuGetAssembly
                            {
                                Assembly = asm,
                                AssemblyName = name,
                                Version = parsedVersion
                            });
                        }
                        catch (Exception ex)
                        {
                            m_Logger.LogError("Failed to load assembly: " + item);
                            m_Logger.LogError(ex.ToString());
                        }
                        finally
                        {
                            ms.Close();
                            stream.Close();
                        }
                    }
                }
            }

            m_LoadedPackageAssemblies.Add(fullPath, assemblies);
            packageReader.Dispose();
            return assemblies.Select(d => d.Assembly);
        }

        public virtual async Task<PackageIdentity> GetLatestPackageIdentityAsync(string packageId)
        {
            if (!Directory.Exists(m_PackagesDirectory))
            {
                return null;
            }

            List<PackageIdentity> packageIdentities = new List<PackageIdentity>();
            foreach (var dir in Directory.GetDirectories(m_PackagesDirectory))
            {
                var dirName = new DirectoryInfo(dir).Name;
                if (dirName.StartsWith(packageId + ".", StringComparison.OrdinalIgnoreCase))
                {
                    var directoryName = new DirectoryInfo(dir).Name;
                    var nupkgFile = Path.Combine(dir, directoryName + ".nupkg");
                    if (!File.Exists(nupkgFile))
                    {
                        return null;
                    }

                    using (var reader = new PackageArchiveReader(nupkgFile))
                    {
                        var identity = await reader.GetIdentityAsync(CancellationToken.None);

                        if (identity.Id.Equals(packageId))
                        {
                            packageIdentities.Add(identity);
                        }
                    }
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

        protected virtual async Task GetPackageDependenciesAsync(PackageIdentity package,
                                                            SourceCacheContext cacheContext,
                                                            IEnumerable<SourceRepository> repositories,
                                                            ISet<SourcePackageDependencyInfo> availablePackages)
        {
            if (availablePackages.Contains(package))
            {
                return;
            }

            m_Logger.LogDebug("GetPackageDependencies: " + package);
            var repos = repositories.ToList();
            m_Logger.LogDebug($"Repos for {package}: " + repos.Count);

            foreach (var sourceRepository in repos)
            {
                m_Logger.LogDebug("GetResourceAsync for " + sourceRepository.PackageSource.SourceUri);
                var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();

                m_Logger.LogDebug("ResolvePackage");
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(package, m_CurrentFramework, cacheContext, m_Logger, CancellationToken.None);

                if (dependencyInfo == null)
                {
                    m_Logger.LogDebug("Dependency was not found: " + package + " in " + sourceRepository.PackageSource.SourceUri);
                    continue;
                }
                m_Logger.LogDebug("Dependency was found: " + package + " in " + sourceRepository.PackageSource.SourceUri);

                availablePackages.Add(dependencyInfo);
                foreach (var dependency in dependencyInfo.Dependencies)
                {
                    await GetPackageDependenciesAsync(
                        new PackageIdentity(dependency.Id, dependency.VersionRange.MaxVersion ?? dependency.VersionRange.MinVersion), cacheContext, repos, availablePackages);
                }
            }
        }

        private void InstallAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
            {
                var name = GetVersionIndependentName(args.Name, out string version);
                var parsedVersion = new Version(version);

                var exactMatch = m_LoadedPackageAssemblies
                                 .Values.SelectMany(d => d)
                                 .FirstOrDefault(d => d.AssemblyName == name && d.Version == parsedVersion);

                if (exactMatch != null)
                {
                    return exactMatch.Assembly;
                }

                var matchingAssemblies =
                    m_LoadedPackageAssemblies.Values.SelectMany(d => d)
                        .Where(d => d.AssemblyName.Equals(name, StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(d => d.Version);

                return matchingAssemblies.FirstOrDefault()?.Assembly;
            };
        }

        private static readonly Regex s_VersionRegex = new Regex("Version=(?<version>.+?), ", RegexOptions.Compiled);
        protected static string GetVersionIndependentName(string fullAssemblyName, out string extractedVersion)
        {
            var match = s_VersionRegex.Match(fullAssemblyName);
            extractedVersion = match.Groups[1].Value;
            return s_VersionRegex.Replace(fullAssemblyName, "");
        }

        public async Task<bool> RemoveAsync(PackageIdentity package)
        {
            var installDir = m_PackagePathResolver.GetInstallPath(package);
            if (installDir == null || !Directory.Exists(installDir))
            {
                return false;
            }

            Directory.Delete(installDir, true);
            return true;
        }
    }
}