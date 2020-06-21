using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("remove")]
    [CommandAlias("-")]
    [CommandAlias("r")]
    [CommandSyntax("<[p]layer/[g]roup> [target] [permission]")]
    [CommandParent(typeof(CommandPermission))]
    public class CommandPermissionRemove : CommandPermissionAction
    {
        private readonly IPermissionChecker m_PermissionChecker;

        public CommandPermissionRemove(IServiceProvider serviceProvider, 
            IPermissionRoleStore permissionRoleStore, 
            IUserDataStore usersDataStore, 
            IPermissionChecker permissionChecker) : base(serviceProvider, permissionRoleStore, usersDataStore)
        {
            m_PermissionChecker = permissionChecker;
        }

        protected override async Task ExecuteUpdateAsync(IPermissionActor target, string roleId)
        {
            var defaultPermissionStore = m_PermissionChecker.PermissionStores.FirstOrDefault(d => d is DefaultPermissionStore);
            if (defaultPermissionStore == null)
            {
                await Context.Actor.PrintMessageAsync($"Failed to remove \"{roleId}\" from \"{target.DisplayName}\": Built-in permission store not found.", Color.Red);
                return;
            }

            if (await defaultPermissionStore.RemoveGrantedPermissionAsync(target, roleId))
            {
                await Context.Actor.PrintMessageAsync($"Removed \"{roleId}\" from \"{target.DisplayName}\".", Color.DarkGreen);
            }
            else
            {
                await Context.Actor.PrintMessageAsync($"\"{target.DisplayName}\" does not have permission \"{roleId}\".", Color.DarkRed);
            }
        }
    }
}