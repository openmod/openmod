using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenMod.API.Localization;
using OpenMod.Core.Plugins.NuGet;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("uninstall")]
    [CommandAlias("ui")]
    [CommandAlias("remove")]
    [CommandAlias("r")]
    [CommandDescription("Uninstall NuGet plugins")]
    [CommandSyntax("<id>")]
    [CommandParent(typeof(CommandOpenMod))]
    public class CommandOpenModRemove : Command
    {
        private readonly IOpenModStringLocalizer m_StringLocalizer;
        private readonly IConfiguration m_Configuration;
        private readonly NuGetPluginAssembliesSource m_NuGetPlugis;

        public CommandOpenModRemove(
            IOpenModStringLocalizer stringLocalizer,
            IConfiguration configuration,
            IServiceProvider serviceProvider, 
            NuGetPluginAssembliesSource nuGetPlugis) : base(serviceProvider)
        {
            m_StringLocalizer = stringLocalizer;
            m_Configuration = configuration;
            m_NuGetPlugis = nuGetPlugis;
        }

        protected override async Task OnExecuteAsync()
        {
            if (Context.Parameters.Length != 1)
            {
                throw new CommandWrongUsageException(Context);
            }

            var allowedActors = m_Configuration.GetValue<List<string>>("nuget:remove:allowedActors");
            if (allowedActors.All(d => d.Trim() != "*" && !Context.Actor.Type.Equals(d.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                throw new UserFriendlyException(this.m_StringLocalizer["commands:openmod:restricted"]);
            }

            string packageName = Context.Parameters[0];

            await Context.Actor.PrintMessageAsync($"Removing {packageName}...", Color.White);
            if (!await m_NuGetPlugis.UninstallPackageAsync(packageName))
            {
                await Context.Actor.PrintMessageAsync($"Failed to remove \"{packageName}\"", Color.DarkRed);
                return;
            }

            await Context.Actor.PrintMessageAsync($"Successfully removed \"{packageName}\"", Color.DarkGreen);
            await Context.Actor.PrintMessageAsync($"To complete deinstallation, please reload OpenMod with /openmod reload.", Color.White);
        }
    }
}