using System;
using System.Threading.Tasks;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("permission", Priority = Priority.Lowest)]
    [CommandAlias("p")]
    [CommandParent(typeof(CommandOpenMod))]
    [CommandDescription("Manage permissions")]
    [CommandSyntax("<add/remove> <[p]layer | [r]ole> <name> <permission>")]
    public class CommandPermission : Command
    {
        public CommandPermission(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override Task OnExecuteAsync()
        {
            return Task.FromException(new CommandWrongUsageException(Context));
        }
    }
}
