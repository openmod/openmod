using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Core.Permissions;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("remove")]
    [CommandAlias("-")]
    [CommandAlias("r")]
    [CommandSyntax("<[p]layer/[r]ole> [target] [permission]")]
    [CommandParent(typeof(CommandPermission))]
    public class CommandPermissionRemove : CommandPermissionAction
    {
        private readonly IPermissionChecker m_PermissionChecker;

        public CommandPermissionRemove(IPermissionChecker permissionChecker,
            ICommandPermissionBuilder commandPermissionBuilder,
            IServiceProvider serviceProvider,
            IPermissionRoleStore permissionRoleStore,
            IUserDataStore usersDataStore,
            IUserManager userManager,
            IPermissionRegistry commandRegistry) : base(serviceProvider, permissionRoleStore, commandPermissionBuilder, permissionChecker, usersDataStore, userManager, commandRegistry)
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
