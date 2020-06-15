using System;
using System.Threading.Tasks;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("permissiongroup")]
    [CommandAlias("pg")]
    [CommandDescription("Manage permission groups")]
    [CommandSyntax("<add/remove> <player | group> <name> <permission group>")]

    public class CommandPermissionGroup : Command
    {
        public CommandPermissionGroup(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override Task OnExecuteAsync()
        {
            return Task.FromException(new CommandWrongUsageException(Context));
        }
    }
}