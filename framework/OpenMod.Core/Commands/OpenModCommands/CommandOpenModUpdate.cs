using System;
using System.Drawing;
using System.Threading.Tasks;
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
        private readonly NuGetPluginAssembliesSource m_NuGetPlugins;

        public CommandOpenModUpdate(IServiceProvider serviceProvider, NuGetPluginAssembliesSource nuGetPlugins) : base(serviceProvider)
        {
            m_NuGetPlugins = nuGetPlugins;
        }

        protected override async Task OnExecuteAsync()
        {
            if(Context.Parameters.Count == 0)
            {
                throw new CommandWrongUsageException(Context);
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