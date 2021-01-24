using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Globbing;
using NuGet.Packaging;
using OpenMod.API.Prioritization;
using OpenMod.NuGet;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("purge", Priority = Priority.Lowest)]
    [CommandParent(typeof(CommandOpenMod))]
    [CommandDescription("Purges and reinstall all OpenMod packages.")]
    [CommandSyntax("[wildcard1] [wildcard2] [wildcardN] [--no-restore]")]
    public class CommandOpenModPurge : Command
    {
        private readonly NuGetPackageManager m_PackageManager;

        public CommandOpenModPurge(
            IServiceProvider serviceProvider,
            NuGetPackageManager packageManager) : base(serviceProvider)
        {
            m_PackageManager = packageManager;
        }

        protected override async Task OnExecuteAsync()
        {
            await PrintAsync("Purging packages...");

            var packagesDirectory = m_PackageManager.PackagesDirectory;

            var options = new GlobOptions { Evaluation = { CaseInsensitive = false } };

            var restore = !Context.Parameters.Contains("--no-restore");
            var filters = Context.Parameters
                .Where(d => d != "--no-restore")
                .Select(d => Glob.Parse(d, options))
                .ToList();

            var i = 0;
            foreach (var packageDir in Directory.GetDirectories(packagesDirectory))
            {
                var nupkg = Directory.GetFiles(packageDir, "*.nupkg", SearchOption.AllDirectories)
                    .FirstOrDefault();

                if (nupkg == null)
                {
                    // Ignore directories that do not have a nupkg file
                    continue;
                }
                try
                {
                    using var reader = new PackageArchiveReader(nupkg);
                    var identity = await reader.GetIdentityAsync(CancellationToken.None);

                    if (filters.Count > 0 && !filters.Any(f => f.IsMatch(identity.Id)))
                    {
                        continue;
                    }

                    Directory.Delete(packageDir, recursive: true);
                    i++;
                }
                catch (Exception ex)
                {
                    var dirName = Path.GetFileName(packageDir);
                    await PrintAsync($"Failed to delete {dirName}: {ex.Message}");
                }
            }

            await PrintAsync($"Purged {i} package(s).");

            if (restore)
            {
                await PrintAsync("Restoring packages...");
                i = await m_PackageManager.InstallMissingPackagesAsync();
                await PrintAsync($"Restored {i} package(s) from packages.yaml.");
            }
        }
    }
}