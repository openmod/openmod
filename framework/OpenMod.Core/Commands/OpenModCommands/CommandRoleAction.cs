using System;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Users;

namespace OpenMod.Core.Commands.OpenModCommands
{
    public abstract class CommandRoleAction : CommandPermissionAction
    {
        private readonly IPermissionRoleStore m_PermissionRoleStore;

        protected CommandRoleAction(IPermissionChecker permissionChecker,
            ICommandPermissionBuilder commandPermissionBuilder,
            IServiceProvider serviceProvider,
            IPermissionRoleStore permissionRoleStore,
            IUserDataStore usersDataStore,
            IUserManager userManager,
            IPermissionRegistry commandRegistry) : base(serviceProvider, permissionRoleStore, commandPermissionBuilder, permissionChecker, usersDataStore, userManager, commandRegistry)
        {
            m_PermissionRoleStore = permissionRoleStore;
        }

        protected override async Task ExecuteUpdateAsync(IPermissionActor target, string roleId)
        {
            var role = await m_PermissionRoleStore.GetRoleAsync(roleId);
            if (role == null)
            {
                throw new UserFriendlyException($"Permission role not found: {roleId}");
            }

            await ExecuteUpdateAsync(target, role);
        }

        protected abstract Task ExecuteUpdateAsync(IPermissionActor target, IPermissionRole permissionRole);
    }
}