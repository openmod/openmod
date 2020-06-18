using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
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

        public CommandPermissionGroupAdd(IServiceProvider serviceProvider, IPermissionGroupStore permissionGroupStore, IUserDataStore userDataStore) : base(serviceProvider, permissionGroupStore, userDataStore)
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