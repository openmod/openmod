using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging.Core;
using OpenMod.NuGet;

namespace OpenMod.Bootstrapper
{
    // NOTE TO DEVELOPER: THIS CLASS SHOULD NOT REFERENCE *ANY* OTHER OPENMOD CODE!
    // OpenMod is not loaded at this sage.
    /// <summary>
    ///     This class is responsible for downloading OpenMod.Core, OpenMod.API, the host package and their dependencies. <br/>
    ///     After download, it will boot OpenMod and then initialize the IOpenModHost interface.
    /// </summary>
    public class OpenModDynamicBootstrapper
    {
        public const string DefaultNugetRepository = "https://api.nuget.org/v3/index.json";

        public async Task BootstrapAsync(
            string openModFolder,
            string[] commandLineArgs,
            string packageId,
            bool allowPrereleaseVersions = false,
            ILogger logger = null)
        {
            await BootstrapAsync(openModFolder, commandLineArgs, new List<string> { packageId }, allowPrereleaseVersions, logger);
        }

        public void Bootstrap(string openModFolder,
                              string[] commandLineArgs,
                              IEnumerable<string> packageIds,
                              bool allowPrereleaseVersions = false,
                              ILogger logger = null)
        {
            BootstrapAsync(openModFolder, commandLineArgs, packageIds, allowPrereleaseVersions, logger).GetAwaiter().GetResult();
        }

        public async Task BootstrapAsync(
            string openModFolder,
            string[] commandLineArgs,
            IEnumerable<string> packageIds,
            bool allowPrereleaseVersions = false,
            ILogger logger = null)
        {
            logger = logger ?? new NuGetConsoleLogger();
            logger.LogInformation("Bootstrap has started.");

            openModFolder = Path.GetFullPath(openModFolder);
            
            var packagesDirectory = Path.Combine(openModFolder, "packages");

            if (!Directory.Exists(packagesDirectory))
            {
                Directory.CreateDirectory(packagesDirectory);
            }

            var nugetInstaller = new NuGetPackageManager(packagesDirectory);
            nugetInstaller.Logger = logger;

            List<Assembly> hostAssemblies = new List<Assembly>();
            foreach (var packageId in packageIds)
            {
                PackageIdentity packageIdentity;
                if (!await nugetInstaller.IsPackageInstalledAsync(packageId))
                {
                    logger.LogInformation("Searching for: " + packageId);
                    var openModPackage = await nugetInstaller.QueryPackageExactAsync(packageId, null, allowPrereleaseVersions);

                    logger.LogInformation($"Downloading {openModPackage.Identity.Id} v{openModPackage.Identity.Version} via NuGet, this might take a while...");
                    var installResult = await nugetInstaller.InstallAsync(openModPackage.Identity, allowPrereleaseVersions);
                    if (installResult.Code != NuGetInstallCode.Success)
                    {
                        logger.LogInformation($"Downloading has failed for {openModPackage.Identity.Id}: " + installResult.Code);
                        return;
                    }

                    packageIdentity = installResult.Identity;
                    logger.LogInformation($"Finished downloading \"{packageId}\"");
                }
                else
                {
                    packageIdentity = await nugetInstaller.GetLatestPackageIdentityAsync(packageId);
                }

                logger.LogInformation($"Loading {packageId}.");
                var packageAssemblies = await LoadPackageAsync(nugetInstaller, packageIdentity);
                hostAssemblies.AddRange(packageAssemblies);
            }

            await InitializeRuntimeAsync(hostAssemblies, openModFolder, commandLineArgs);
        }

        private async Task<IEnumerable<Assembly>> LoadPackageAsync(NuGetPackageManager packageManager, PackageIdentity identity)
        {
            var pkg = packageManager.GetNugetPackageFile(identity);
            return await packageManager.LoadAssembliesFromNuGetPackageAsync(pkg);
        }

        private Task InitializeRuntimeAsync(List<Assembly> hostAssemblies, string workingDirectory, string[] commandlineArgs)
        {
            var runtimeAssembly = hostAssemblies.FirstOrDefault(d => d.GetName().Name.Equals("OpenMod.Runtime"));
            var apiAssembly = hostAssemblies.FirstOrDefault(d => d.GetName().Name.Equals("OpenMod.API"));
            var runtimeType = runtimeAssembly.GetType("OpenMod.Runtime.Runtime");
            var runtime = Activator.CreateInstance(runtimeType);

            var parametersType = apiAssembly.GetType("OpenMod.API.RuntimeInitParameters");
            var parameters = Activator.CreateInstance(parametersType);
            SetParameter(parameters, "WorkingDirectory", workingDirectory);
            SetParameter(parameters, "CommandlineArgs", commandlineArgs);
            
            var initMethod = runtimeType.GetMethod("InitAsync", BindingFlags.Instance | BindingFlags.Public);
            return (Task) initMethod.Invoke(runtime, new object[] {  hostAssemblies, null /* hostBuilder */, parameters});
        }

        private void SetParameter(object parametersObject, string name, object value)
        {
            var field = parametersObject.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public);
            field.SetValue(parametersObject, value);
        }
    }
}