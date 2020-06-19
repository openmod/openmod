using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Nito.AsyncEx;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using OpenMod.NuGet;

namespace OpenMod.Bootstrapper
{
    // NOTE TO DEVELOPER: THIS CLASS SHOULD NOT REFERENCE *ANY* OTHER OPENMOD CODE!
    // OpenMod is not loaded at this stage (except for OpenMod.NuGet, which has no dependencies).
    /// <summary>
    ///     This class is responsible for downloading OpenMod.Core, OpenMod.API, the host package and their dependencies. <br/>
    ///     After download, it will boot OpenMod and then initialize the IRuntime implementation.
    /// </summary>
    public class OpenModDynamicBootstrapper
    {
        public const string DefaultNugetRepository = "https://api.nuget.org/v3/index.json";

        public Task BootstrapAsync(
            string openModFolder,
            string[] commandLineArgs,
            string packageId,
            bool allowPrereleaseVersions = false,
            ILogger logger = null)
        {
            return BootstrapAsync(openModFolder, commandLineArgs, new List<string> { packageId }, allowPrereleaseVersions, logger);
        }

        public void Bootstrap(string openModFolder,
                              string[] commandLineArgs,
                              IEnumerable<string> packageIds,
                              bool allowPrereleaseVersions = false,
                              ILogger logger = null)
        {
            AsyncContext.Run(() => BootstrapAsync(openModFolder, commandLineArgs, packageIds, allowPrereleaseVersions, logger));
        }

        public async Task BootstrapAsync(
            string openModFolder,
            string[] commandLineArgs,
            IEnumerable<string> packageIds,
            bool allowPrereleaseVersions = false,
            ILogger logger = null)
        {
            var shouldAutoUpdate = false;
            if (!commandLineArgs.Any(arg => arg.Equals("-NoOpenModAutoUpdate", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (!bool.TryParse(Environment.GetEnvironmentVariable("OpenMod__AutoUpdate"), out shouldAutoUpdate))
                    shouldAutoUpdate = true;
            }


            logger ??= new NuGetConsoleLogger();
            openModFolder = Path.GetFullPath(openModFolder);
            
            var packagesDirectory = Path.Combine(openModFolder, "packages");

            if (!Directory.Exists(packagesDirectory))
            {
                Directory.CreateDirectory(packagesDirectory);
            }

            var nugetInstaller = new NuGetPackageManager(packagesDirectory) {Logger = logger};

            var hostAssemblies = new List<Assembly>();
            foreach (var packageId in packageIds)
            {
                var packageIdentity = await nugetInstaller.GetLatestPackageIdentityAsync(packageId);
                var shouldInstallOrUpdate = packageIdentity == null;

                IPackageSearchMetadata openModPackage = null;
                if (packageIdentity == null || shouldAutoUpdate)
                {
                    openModPackage = await nugetInstaller.QueryPackageExactAsync(packageId, null, allowPrereleaseVersions);
                }

                if (packageIdentity != null && shouldAutoUpdate)
                {
                    var availableVersions = await openModPackage.GetVersionsAsync();
                    shouldInstallOrUpdate = availableVersions.Any(d => d.Version > packageIdentity.Version);
                }

                if (shouldInstallOrUpdate)
                {
                    logger.LogInformation($"Downloading {openModPackage.Identity.Id} v{openModPackage.Identity.Version} via NuGet");
                    var installResult = await nugetInstaller.InstallAsync(openModPackage.Identity, allowPrereleaseVersions);
                    if (installResult.Code == NuGetInstallCode.Success)
                    {
                        packageIdentity = installResult.Identity;
                        logger.LogInformation($"Finished downloading \"{packageId}\".");
                    }
                    else
                    {
                        logger.LogError($"Downloading has failed for {openModPackage.Identity.Id} v{openModPackage.Identity.Version.OriginalVersion}: {installResult.Code}");
                        if (packageIdentity == null)
                        {
                            return;
                        }
                    }
                }

                logger.LogInformation($"Loading {packageIdentity.Id} v{packageIdentity.Version}");
                var packageAssemblies = await LoadPackageAsync(nugetInstaller, packageIdentity);
                hostAssemblies.AddRange(packageAssemblies);
            }

            await InitializeRuntimeAsync(hostAssemblies, openModFolder, commandLineArgs);
        }

        
        private Task<IEnumerable<Assembly>> LoadPackageAsync(NuGetPackageManager packageManager, PackageIdentity identity)
        {
            var pkg = packageManager.GetNugetPackageFile(identity);
            return packageManager.LoadAssembliesFromNuGetPackageAsync(pkg);
        }

        private Task InitializeRuntimeAsync(List<Assembly> hostAssemblies, string workingDirectory, string[] commandlineArgs)
        {
            var runtimeAssembly = FindAssemblyInCurrentDomain("OpenMod.Runtime");
            var apiAssembly = FindAssemblyInCurrentDomain("OpenMod.API");
            var runtimeType = runtimeAssembly.GetType("OpenMod.Runtime.Runtime");
            var runtime = Activator.CreateInstance(runtimeType);

            var parametersType = apiAssembly.GetType("OpenMod.API.RuntimeInitParameters");
            var parameters = Activator.CreateInstance(parametersType);
            SetParameter(parameters, "WorkingDirectory", workingDirectory);
            SetParameter(parameters, "CommandlineArgs", commandlineArgs);
            
            var initMethod = runtimeType.GetMethod("InitAsync", BindingFlags.Instance | BindingFlags.Public);
            return (Task) initMethod?.Invoke(runtime, new[] {  hostAssemblies, parameters, null /* hostBuilderFunc */});
        }

        
        private Assembly FindAssemblyInCurrentDomain(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(d => d.GetName().Name.Equals(name)) ??
                   throw new Exception($"Failed to find assembly: {name}");
        }

        
        private void SetParameter(object parametersObject, string name, object value)
        {
            var field = parametersObject.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public);
            field?.SetValue(parametersObject, value);
        }
    }
}
