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
    [CommandSyntax("<[p]layer/[g]roup> [target] [role]")]
    [CommandParent(typeof(CommandRole))]
    public class CommandRoleAdd : CommandRoleAction
    {
        private readonly IPermissionRoleStore m_PermissionRoleStore;

        public CommandRoleAdd(IServiceProvider serviceProvider, IPermissionRoleStore permissionRoleStore, IUserDataStore userDataStore) : base(serviceProvider, permissionRoleStore, userDataStore)
        {
            m_PermissionRoleStore = permissionRoleStore;
        }

        protected override async Task ExecuteUpdateAsync(IPermissionActor target, IPermissionRole permissionRole)
        {
            if (await m_PermissionRoleStore.AddRoleToActorAsync(target, permissionRole.Id))
            {
                await Context.Actor.PrintMessageAsync($"Added \"{target.DisplayName}\" to \"{permissionRole.DisplayName}\".", Color.DarkGreen);

            }
            else
            {
                await Context.Actor.PrintMessageAsync($"Failed to add \"{target.DisplayName}\" to \"{permissionRole.DisplayName}\".", Color.DarkRed);
            }
        }
    }
}