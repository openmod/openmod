using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.Core.Users;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("remove")]
    [CommandAlias("-")]
    [CommandAlias("r")]
    [CommandSyntax("<[p]layer/[g]roup> [target] [group]")]
    [CommandParent(typeof(CommandPermissionGroup))]
    public class CommandPermissionGroupRemove : CommandPermissionGroupAction
    {
        private readonly IPermissionGroupStore m_PermissionGroupStore;

        public CommandPermissionGroupRemove(IServiceProvider serviceProvider, IPermissionGroupStore permissionGroupStore, IUsersDataStore usersDataStore) : base(serviceProvider, permissionGroupStore, usersDataStore)
        {
            m_PermissionGroupStore = permissionGroupStore;
        }

        protected override async Task ExecuteUpdateAsync(IPermissionActor target, IPermissionGroup permissionGroup)
        {
            if (await m_PermissionGroupStore.RemoveGroupFromActorAsync(target, permissionGroup.Id))
            {
                await Context.Actor.PrintMessageAsync($"Removed \"{target.DisplayName}\" from \"{permissionGroup.DisplayName}\".", Color.DarkGreen);

            }
            else
            {
                await Context.Actor.PrintMessageAsync($"Failed to remove \"{target.DisplayName}\" from \"{permissionGroup.DisplayName}\".", Color.DarkRed);
            }
        }
    }
}