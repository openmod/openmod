using System;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("reload", Priority = Priority.Highest)]
    [CommandDescription("Reloads OpenMod")]
    [CommandSyntax("[--hard]")]
    [CommandParent(typeof(CommandOpenMod))]
    public class CommandOpenModReload : Command
    {
        private readonly IRuntime m_Runtime;
        private readonly IOpenModHost m_OpenModHost;

        public CommandOpenModReload(IServiceProvider serviceProvider,
            IRuntime runtime,
            IOpenModHost openModHost) : base(serviceProvider)
        {
            m_Runtime = runtime;
            m_OpenModHost = openModHost;
        }

        protected override async Task OnExecuteAsync()
        {
            await Context.Actor.PrintMessageAsync("Reloading OpenMod, this might take a while...");
            var isHardReload = Context.Parameters.Contains("--hard");
            if (isHardReload)
            {
                await m_OpenModHost.PerformHardReloadAsync();
            }
            else
            {
                await m_Runtime.PerformSoftReloadAsync();
            }

            await Context.Actor.PrintMessageAsync("OpenMod has been reloaded.");
        }
    }
}