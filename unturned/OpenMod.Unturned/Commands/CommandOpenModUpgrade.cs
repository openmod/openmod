using Newtonsoft.Json;
using OpenMod.API;
using OpenMod.Common.Hotloading;
using OpenMod.Core.Commands;
using OpenMod.Core.Commands.OpenModCommands;
using SDG.Framework.Modules;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.NuGet;
using Command = OpenMod.Core.Commands.Command;

namespace OpenMod.Unturned.Commands
{
    [Command("upgrade")]
    [CommandDescription("Upgrades OpenMod")]
    [CommandParent(typeof(CommandOpenMod))]
    public class CommandOpenModUpgrade : Command
    {
        private readonly IRuntime m_Runtime;
        private readonly NuGetPackageManager m_PackageManager;
        private readonly IHostInformation m_HostInformation;
        private readonly ILogger<CommandOpenModUpgrade> m_Logger;

        public CommandOpenModUpgrade(
            IServiceProvider serviceProvider,
            IRuntime runtime,
            NuGetPackageManager packageManager,
            IHostInformation hostInformation,
            ILogger<CommandOpenModUpgrade> logger) : base(serviceProvider)
        {
            m_Runtime = runtime;
            m_PackageManager = packageManager;
            m_HostInformation = hostInformation;
            m_Logger = logger;
        }

        private static readonly string[] s_IgnoredNameFiles =
        {
            "OpenMod.Unturned.Module.Bootstrapper.dll",
            "Readme.txt"
        };


        protected override async Task OnExecuteAsync()
        {
            var modulesDirectory = Path.Combine(ReadWrite.PATH, "Modules");
            var openModDirPath = Path.GetDirectoryName(Directory
                .GetFiles(modulesDirectory, "OpenMod.Unturned.Module.dll", SearchOption.AllDirectories)
                .FirstOrDefault() ?? throw new Exception("Failed to find OpenMod directory"))!;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "request");

            var releaseData = await client.GetStringAsync("https://api.github.com/repos/openmod/openmod/releases/latest");
            var release = JsonConvert.DeserializeObject<LatestRelease>(releaseData);
            var isPre = Context.Parameters.Contains("--pre");
            var anyUpdated = false;

            var moduleAsset = release.Assets.Find(x => x.BrowserDownloadUrl.Contains("OpenMod.Unturned.Module"));
            if (moduleAsset != null && m_HostInformation.HostVersion.CompareTo(release.TagName) < 0)
            {
                BackupFiles(openModDirPath);

                try
                {
                    await PrintAsync($"Downloading {moduleAsset.AssetName}...");

                    var stream = await client.GetStreamAsync(moduleAsset.BrowserDownloadUrl);

                    await PrintAsync("Extracting update...");
                    await ExtractArchiveAsync(stream, openModDirPath!);
                }
                catch (Exception)
                {
                    RestoreFiles(openModDirPath);
                    throw;
                }

                DeleteBackup(openModDirPath);
                anyUpdated = true;
            }

            foreach (var assembly in m_Runtime.HostAssemblies)
            {
                var ident = await m_PackageManager.QueryPackageExactAsync(assembly.GetName().Name, null, isPre);
                if (ident == null)
                {
                    m_Logger.LogWarning("No package found for assembly: {AssemblyName}", assembly.FullName);
                    continue;
                }

                var installed = await m_PackageManager.GetLatestPackageIdentityAsync(ident.Identity.Id);
                if (installed != null && installed.Version >= ident.Identity.Version)
                {
                    continue;
                }

                await PrintAsync($"Updating package \"{ident.Identity.Id}\" to {ident.Identity.Version}");
                await m_PackageManager.InstallAsync(ident.Identity, isPre);
                anyUpdated = true;
            }

            if (!anyUpdated)
            {
                await PrintAsync("No update found.");
                return;
            }

            if (Hotloader.Enabled)
            {
                var modulePath = Path.Combine(openModDirPath, "OpenMod.Unturned.Module.dll");
                var moduleAssembly = Hotloader.LoadAssembly(File.ReadAllBytes(modulePath));
                var moduleNexusType = moduleAssembly.GetType("OpenMod.Unturned.Module.OpenModUnturnedModule");

                if (moduleNexusType == null)
                {
                    await PrintAsync("Failed to dynamically reload OpenMod. Please restart your server.",
                        Color.Red);
                    return;
                }

                await PrintAsync("Reloading OpenMod...");

                try
                {
                    await m_Runtime.ShutdownAsync();

                    var nexus = (IModuleNexus)Activator.CreateInstance(moduleNexusType);

                    // set OpenModUnturnedModule.IsDynamicLoad to true
                    var isDynamicPropertySetter = moduleNexusType.GetProperty("IsDynamicLoad")?.GetSetMethod(true);
                    isDynamicPropertySetter?.Invoke(nexus, new object[] { true });

                    nexus.initialize();
                }
                catch
                {
                    await PrintAsync(
                        "Reloading OpenMod has failed! Please restart your server and report this error.",
                        Color.Red);
                    throw;
                }

                await PrintAsync("Update has been installed and loaded.");
            }
            else
            {
                await PrintAsync("Update has been installed. Restart to apply it.");
            }

        }

        private void DeleteBackup(string openModDirPath)
        {
            foreach (var file in Directory.GetFiles(openModDirPath, "*.bak"))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    m_Logger.LogDebug(ex, "Exception occurred while deleting backup");
                }
            }
        }

        private void RestoreFiles(string openModDirPath)
        {
            foreach (var file in Directory.GetFiles(openModDirPath))
            {
                if (file.EndsWith(".bak"))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        m_Logger.LogDebug(ex, "Exception occurred while restoring backup");
                    }
                }
            }

            foreach (var file in Directory.GetFiles(openModDirPath))
            {
                if (file.EndsWith(".bak"))
                {
                    try
                    {
                        File.Move(file, file.Replace(".bak", string.Empty));
                    }
                    catch (Exception ex)
                    {
                        m_Logger.LogError(ex, "Exception occurred while restoring backup");
                    }
                }
            }
        }

        private void BackupFiles(string openModDirPath)
        {
            foreach (var file in Directory.GetFiles(openModDirPath))
            {
                if (s_IgnoredNameFiles.Contains(Path.GetFileName(file)))
                {
                    continue;
                }

                try
                {
                    File.Move(file, file + ".bak");
                }
                catch (Exception ex)
                {
                    m_Logger.LogDebug(ex, "Exception occurred while creating backup");
                }
            }
        }

        private async Task ExtractArchiveAsync(Stream archiveStream, string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var zip = new ZipArchive(archiveStream);
            foreach (var file in zip.Entries)
            {
                if (string.IsNullOrEmpty(file.Name)
                    || s_IgnoredNameFiles.Contains(file.Name))
                {
                    continue;
                }

                var path = Path.Combine(directory, file.Name);

                using var fileStream = file.Open();
                using var ms = new MemoryStream();

                await fileStream.CopyToAsync(ms);
                ms.Seek(offset: 0, SeekOrigin.Begin);

                File.WriteAllBytes(path, ms.ToArray());
            }
        }

        private class LatestRelease
        {
            [JsonProperty("assets")]
            public List<Asset> Assets { get; set; } = null!;

            [JsonProperty("tag_name")]
            public string TagName { get; set; } = null!;
        }

        private class Asset
        {
            [JsonProperty("name")]
            public string AssetName { get; set; } = null!;

            [JsonProperty("browser_download_url")]
            public string BrowserDownloadUrl { get; set; } = null!;
        }
    }
}