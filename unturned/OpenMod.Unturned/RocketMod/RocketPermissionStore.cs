using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Permissions;

namespace OpenMod.Unturned.RocketMod
{
    public class RocketPermissionStore : IPermissionStore
    {
        /// <inheritdoc />
        public Task<IReadOnlyCollection<string>> GetGrantedPermissionsAsync(IPermissionActor actor, bool inherit = true)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<string>> GetDeniedPermissionsAsync(IPermissionActor actor, bool inherit = true)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> AddGrantedPermissionAsync(IPermissionActor actor, string permission)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> AddDeniedPermissionAsync(IPermissionActor actor, string permission)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> RemoveGrantedPermissionAsync(IPermissionActor actor, string permission)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> RemoveDeniedPermissionAsync(IPermissionActor actor, string permission)
        {
            throw new System.NotImplementedException();
        }
    }
}