using System;
using System.Drawing;
using System.Threading.Tasks;
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
        private readonly NuGetPluginAssembliesSource m_NuGetPlugis;

        public CommandOpenModRemove(IServiceProvider serviceProvider, NuGetPluginAssembliesSource nuGetPlugis) : base(serviceProvider)
        {
            m_NuGetPlugis = nuGetPlugis;
        }

        protected override async Task OnExecuteAsync()
        {
            if (Context.Parameters.Length != 1)
            {
                throw new CommandWrongUsageException(Context);
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