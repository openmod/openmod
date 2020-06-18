using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenMod.API.Localization;
using OpenMod.Core.Plugins.NuGet;
using OpenMod.NuGet;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("update")]
    [CommandAlias("u")]
    [CommandDescription("Update plugins from NuGet")]
    [CommandSyntax("<id> [version] [-Pre]")]
    [CommandParent(typeof(CommandOpenMod))]
    public class CommandOpenModUpdate : Command
    {
        private readonly IConfiguration m_Configuration;
        private readonly IOpenModStringLocalizer m_StringLocalizer;
        private readonly NuGetPluginAssembliesSource m_NuGetPlugins;

        public CommandOpenModUpdate(
            IConfiguration configuration,
            IOpenModStringLocalizer stringLocalizer,
            IServiceProvider serviceProvider, NuGetPluginAssembliesSource nuGetPlugins) : base(serviceProvider)
        {
            m_Configuration = configuration;
            m_StringLocalizer = stringLocalizer;
            m_NuGetPlugins = nuGetPlugins;
        }

        protected override async Task OnExecuteAsync()
        {
            if(Context.Parameters.Count == 0)
            {
                throw new CommandWrongUsageException(Context);
            }

            var allowedActors = m_Configuration.GetValue<List<string>>("nuget:update:allowedActors");
            if (allowedActors.All(d => d.Trim() != "*" && !Context.Actor.Type.Equals(d.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                throw new UserFriendlyException(m_StringLocalizer["commands:openmod:restricted"]);
            }

            var args = Context.Parameters.ToList();

            string packageName = Context.Parameters[0];
            string version = null;
            bool isPre = false;

            if (args.Contains("-Pre"))
            {
                isPre = true;
                args.Remove("-Pre");
            }

            if (args.Count > 1)
            {
                version = args[1];
            }

            await Context.Actor.PrintMessageAsync($"Updating {packageName}...", Color.White);

            var result = await m_NuGetPlugins.UpdatePackageAsync(packageName, version, isPre);
            if (result.Code != NuGetInstallCode.Success)
            {
                await Context.Actor.PrintMessageAsync($"Failed to update \"{packageName}\": " + result.Code, Color.DarkRed);
                return;
            }

            await Context.Actor.PrintMessageAsync($"Successfully updated {result.Identity.Id} v{result.Identity.Version}.", Color.DarkGreen);
            await Context.Actor.PrintMessageAsync($"To complete update, please reload OpenMod with /openmod reload.", Color.White);
        }
    }
}