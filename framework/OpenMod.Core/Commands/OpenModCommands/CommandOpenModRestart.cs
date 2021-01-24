using System;
using System.Threading.Tasks;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("restart")]
    [CommandParent(typeof(CommandOpenMod))]
    [CommandDescription("Restarts the server.")]
    public class CommandOpenModRestart : Command
    {
        public CommandOpenModRestart(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async Task OnExecuteAsync()
        {
            await PrintAsync("This command is currently not implemented yet.");
        }
    }
}