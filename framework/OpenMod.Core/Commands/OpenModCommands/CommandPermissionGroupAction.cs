using System;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Core.Users;

namespace OpenMod.Core.Commands.OpenModCommands
{
    public abstract class CommandPermissionGroupAction : CommandPermissionAction
    {
        private readonly IPermissionGroupStore m_PermissionGroupStore;

        protected CommandPermissionGroupAction(IServiceProvider serviceProvider, IPermissionGroupStore permissionGroupStore, IUserDataStore userDataStore) : base(serviceProvider, permissionGroupStore, userDataStore)
        {
            m_PermissionGroupStore = permissionGroupStore;
        }

        protected override async Task ExecuteUpdateAsync(IPermissionActor target, string groupId)
        {
            var permissionGroup = await m_PermissionGroupStore.GetGroupAsync(groupId);
            if (permissionGroup == null)
            {
                throw new UserFriendlyException($"Permission group not found: {groupId}");
            }

            await ExecuteUpdateAsync(target, permissionGroup);
        }

        protected abstract Task ExecuteUpdateAsync(IPermissionActor target, IPermissionGroup permissionGroup);
    }
}