using Microsoft.Extensions.Logging;
using OpenMod.Common.Hotloading;
using OpenMod.NuGet;
using System;
using System.IO;
using System.Reflection;

namespace OpenMod.EntityFrameworkCore.MySql
{
    public sealed class PomeloMySqlConnectorResolver
    {
        public PomeloMySqlConnectorResolver(NuGetPackageManager nuGetPackageManager,
            ILogger<PomeloMySqlConnectorResolver> logger)
        {
            if (!Hotloader.Enabled)
            {
                logger.LogCritical(
                    "Hotloader is not enabled and MySqlConnector v0.69 will not be able to load. Some plugins may not work as intended.");
                return;
            }

            using var stream = GetType().Assembly
                .GetManifestResourceStream("OpenMod.EntityFrameworkCore.MySql.libs.MySqlConnector.dll");

            if (stream == null)
            {
                logger.LogCritical(
                    "Could not retrieve MySqlConnector assembly stream. Some plugins may not work as intended.");
                return;
            }

            using var memStream = new MemoryStream();

            stream.CopyTo(memStream);

            var assemblyBytes = memStream.ToArray();

            Hotloader.LoadAssembly(assemblyBytes);

            logger.LogDebug("Loaded MySqlConnector v0.69 into Hotloader");


            if (!nuGetPackageManager.AssemblyResolverInstalled)
            {
                return;
            }

            var assemblyResolveMethod = typeof(NuGetPackageManager).GetMethod("OnAssemblyResolve",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (assemblyResolveMethod == null)
            {
                logger.LogCritical($"Couldn't find OnAssemblyResolve method for {nameof(NuGetPackageManager)}!");
            }
            else
            {
                var @delegate = (ResolveEventHandler)assemblyResolveMethod.CreateDelegate(typeof(ResolveEventHandler), nuGetPackageManager);

                AppDomain.CurrentDomain.AssemblyResolve -= @delegate;
                AppDomain.CurrentDomain.AssemblyResolve += @delegate;
            }
        }
    }
}
