using System;
using System.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("reload", Priority = Priority.Highest)]
    [CommandDescription("Reloads OpenMod")]
    [CommandParent(typeof(CommandOpenMod))]
    public class CommandOpenModReload : Command
    {
        private readonly IRuntime m_Runtime;

        public CommandOpenModReload(IServiceProvider serviceProvider, IRuntime runtime) : base(serviceProvider)
        {
            m_Runtime = runtime;
        }

        protected override async Task OnExecuteAsync()
        {
            await Context.Actor.PrintMessageAsync("Reloading OpenMod, this might take a while...");
            await m_Runtime.ReloadAsync();
            await Context.Actor.PrintMessageAsync("OpenMod has been reloaded.");
        }
    }
}