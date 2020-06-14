using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.Core.Users;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("add")]
    [CommandAlias("+")]
    [CommandAlias("a")]
    [CommandSyntax("<[p]layer/[g]roup> [target] [group]")]
    [CommandParent(typeof(CommandPermissionGroup))]
    public class CommandPermissionGroupAdd : CommandPermissionGroupAction
    {
        private readonly IPermissionGroupStore m_PermissionGroupStore;

        public CommandPermissionGroupAdd(IServiceProvider serviceProvider, IPermissionGroupStore permissionGroupStore, IUsersDataStore usersDataStore) : base(serviceProvider, permissionGroupStore, usersDataStore)
        {
            m_PermissionGroupStore = permissionGroupStore;
        }

        protected override async Task ExecuteUpdateAsync(IPermissionActor target, IPermissionGroup permissionGroup)
        {
            if (await m_PermissionGroupStore.AddGroupToActorAsync(target, permissionGroup.Id))
            {
                await Context.Actor.PrintMessageAsync($"Added \"{target.DisplayName}\" to \"{permissionGroup.DisplayName}\".", Color.DarkGreen);

            }
            else
            {
                await Context.Actor.PrintMessageAsync($"Failed to add \"{target.DisplayName}\" to \"{permissionGroup.DisplayName}\".", Color.DarkRed);
            }
        }
    }
}