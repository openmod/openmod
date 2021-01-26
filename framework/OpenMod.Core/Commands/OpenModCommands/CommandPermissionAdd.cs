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
    [Command("add")]
    [CommandAlias("+")]
    [CommandAlias("a")]
    [CommandSyntax("<[p]layer/[r]ole> [target] [permission]")]
    [CommandParent(typeof(CommandPermission))]
    public class CommandPermissionAdd : CommandPermissionAction
    {
        private readonly IPermissionChecker m_PermissionChecker;

        public CommandPermissionAdd(IPermissionChecker permissionChecker,
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
                await Context.Actor.PrintMessageAsync($"Failed to add \"{roleId}\" to \"{target.DisplayName}\": Built-in permission store not found.", Color.Red);
                return;
            }

            await defaultPermissionStore.AddGrantedPermissionAsync(target, roleId);
            await Context.Actor.PrintMessageAsync($"Added \"{roleId}\" to \"{target.DisplayName}\".", Color.DarkGreen);
        }
    }
}
