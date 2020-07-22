using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Core.Users;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("remove")]
    [CommandAlias("-")]
    [CommandAlias("r")]
    [CommandSyntax("<[p]layer/[r]ole> [target] [role]")]
    [CommandParent(typeof(CommandRole))]
    public class CommandRoleRemove : CommandRoleAction
    {
        private readonly IPermissionRoleStore m_PermissionRoleStore;

        public CommandRoleRemove(IServiceProvider serviceProvider, IPermissionRoleStore permissionRoleStore, IUserDataStore userDataStore, IUserManager userManager) : base(serviceProvider, permissionRoleStore, userDataStore, userManager)
        {
            m_PermissionRoleStore = permissionRoleStore;
        }

        protected override async Task ExecuteUpdateAsync(IPermissionActor target, IPermissionRole permissionRole)
        {
            if (await m_PermissionRoleStore.RemoveRoleFromActorAsync(target, permissionRole.Id))
            {
                await Context.Actor.PrintMessageAsync($"Removed \"{target.DisplayName}\" from \"{permissionRole.DisplayName}\".", Color.DarkGreen);
            }
            else
            {
                await Context.Actor.PrintMessageAsync($"Failed to remove \"{target.DisplayName}\" from \"{permissionRole.DisplayName}\".", Color.DarkRed);
            }
        }
    }
}
