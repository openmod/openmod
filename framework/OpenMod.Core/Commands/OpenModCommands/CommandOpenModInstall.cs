using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenMod.API.Commands;
using OpenMod.API.Localization;
using OpenMod.API.Prioritization;
using OpenMod.Common.Hotloading;
using OpenMod.Core.Plugins.NuGet;
using OpenMod.NuGet;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("install", Priority = Priority.High)]
    [CommandAlias("i")]
    [CommandDescription("Install plugins from NuGet")]
    [CommandSyntax("<id[@version]> [-Pre]")]
    [CommandParent(typeof(CommandOpenMod))]
    public class CommandOpenModInstall : Command
    {
        private readonly IOpenModStringLocalizer m_StringLocalizer;
        private readonly IConfiguration m_Configuration;
        private readonly NuGetPluginAssembliesSource m_NuGetPlugins;

        public CommandOpenModInstall(
            IOpenModStringLocalizer stringLocalizer,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            NuGetPluginAssembliesSource nuGetPlugins) : base(serviceProvider)
        {
            m_StringLocalizer = stringLocalizer;
            m_Configuration = configuration;
            m_NuGetPlugins = nuGetPlugins;
        }

        protected override async Task OnExecuteAsync()
        {
            if (Context.Parameters.Count == 0)
            {
                throw new CommandWrongUsageException(Context);
            }

            var allowedActors = m_Configuration.GetSection("nuget:install:allowedActors").Get<string[]>();
            if (allowedActors.All(d =>
                d.Trim() != "*" && !Context.Actor.Type.Equals(d.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                throw new UserFriendlyException(m_StringLocalizer["commands:openmod:restricted"]!);
            }

            var args = Context.Parameters.ToList();
            var isPre = args.Remove("-Pre");

            var anySuccessful = false;
            foreach (var arg in args)
            {
                if (string.IsNullOrWhiteSpace(arg))
                {
                    throw new CommandWrongUsageException(Context);
                }

                var packageInfo = arg.Split('@');
                var packageName = packageInfo[0];
                var packageVersion = packageInfo.Length > 1 ? packageInfo[1] : null;

                if (packageVersion?.Equals("latest", StringComparison.OrdinalIgnoreCase) == true)
                {
                    packageVersion = null;
                }

                await PrintAsync($"Installing {arg}...", Color.White);
                var result = await m_NuGetPlugins.InstallPackageAsync(packageName, packageVersion, isPre);

                switch (result.Code)
                {
                    case NuGetInstallCode.Success:
                        await PrintAsync(
                            $"Successfully installed {result.Identity!.Id} v{result.Identity!.Version}.", Color.DarkGreen);
                        anySuccessful = true;
                        break;
                    case NuGetInstallCode.NoUpdatesFound:
                        await PrintAsync(
                            $"No updates found for {result.Identity!.Id}.", Color.DarkGreen);
                        break;
                    case NuGetInstallCode.InvalidVersion:
                        await PrintAsync($"Failed to install \"{packageName}@{packageVersion}\": " + result.Code, Color.DarkRed);
                        break;
                    default:
                        await PrintAsync($"Failed to install \"{packageName}\": " + result.Code, Color.DarkRed);
                        break;
                }
            }

            if (anySuccessful)
            {
                if (Hotloader.Enabled)
                {
                    await PrintAsync("To complete installation, please reload OpenMod with /openmod reload.", Color.White);
                }
                else
                {
                    await PrintAsync("To complete installation, please restart your server or enable hotloading and reload.", Color.White);
                }
            }
        }
    }
}