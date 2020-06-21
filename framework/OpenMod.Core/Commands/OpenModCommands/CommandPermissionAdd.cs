using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("add")]
    [CommandAlias("+")]
    [CommandAlias("a")]
    [CommandSyntax("<[p]layer/[g]roup> [target] [permission]")]
    [CommandParent(typeof(CommandPermission))]
    public class CommandPermissionAdd : CommandPermissionAction
    {
        private readonly IPermissionChecker m_PermissionChecker;

        public CommandPermissionAdd(
            IPermissionChecker permissionChecker,
            IServiceProvider serviceProvider,
            IPermissionRoleStore permissionRoleStore,
            IUserDataStore usersDataStore) : base(serviceProvider, permissionRoleStore, usersDataStore)
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