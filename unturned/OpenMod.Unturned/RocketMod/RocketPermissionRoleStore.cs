using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Permissions;

namespace OpenMod.Unturned.RocketMod
{
    public class RocketPermissionRoleStore : IPermissionRoleStore
    {
        private readonly IPermissionRoleStore m_BasePermissionRoleStore;

        public RocketPermissionRoleStore(IPermissionRoleStore basePermissionRoleStore)
        {
            m_BasePermissionRoleStore = basePermissionRoleStore;
        }

        public Task<IReadOnlyCollection<IPermissionRole>> GetRolesAsync(IPermissionActor actor, bool inherit = true)
        {
            throw new System.NotImplementedException();
        }

        public Task<IReadOnlyCollection<IPermissionRole>> GetRolesAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IPermissionRole> GetRoleAsync(string id)
        {
            if (IsRocketModRole(id))
            {
                throw new System.NotImplementedException();
            }

            return await m_BasePermissionRoleStore.GetRoleAsync(id);
        }

        public async Task<bool> UpdateRoleAsync(IPermissionRole role)
        {
            if (IsRocketModRole(role.Id))
            {
                throw new System.NotSupportedException();
            }

            return await m_BasePermissionRoleStore.UpdateRoleAsync(role);

        }

        public async Task<bool> AddRoleToActorAsync(IPermissionActor actor, string roleId)
        {
            if (IsRocketModRole(roleId))
            {
                throw new System.NotSupportedException();
            }

            return await m_BasePermissionRoleStore.AddRoleToActorAsync(actor, roleId);
        }

        public async Task<bool> RemoveRoleFromActorAsync(IPermissionActor actor, string roleId)
        {
            if (IsRocketModRole(roleId))
            {
                throw new System.NotSupportedException();
            }

            return await m_BasePermissionRoleStore.RemoveRoleFromActorAsync(actor, roleId);
        }

        public Task<bool> CreateRoleAsync(IPermissionRole role)
        {
            return m_BasePermissionRoleStore.CreateRoleAsync(role);
        }

        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            if (IsRocketModRole(roleId))
            {
                throw new System.NotSupportedException();
            }

            return await m_BasePermissionRoleStore.DeleteRoleAsync(roleId);
        }

        public Task<IReadOnlyCollection<string>> GetAutoAssignedRolesAsync(string actorId, string actorType)
        {
            throw new System.NotImplementedException();
        }

        public async Task SavePersistentDataAsync<T>(string roleId, string key, T data)
        {
            if (IsRocketModRole(roleId))
            {
                throw new System.NotSupportedException();
            }

            await m_BasePermissionRoleStore.SavePersistentDataAsync<T>(roleId, key, data);
        }

        /// <inheritdoc />
        public async Task<T> GetPersistentDataAsync<T>(string roleId, string key)
        {
            if (IsRocketModRole(roleId))
            {
                throw new System.NotSupportedException();
            }

            return await GetPersistentDataAsync<T>(roleId, key);
        }

        private static bool IsRocketModRole(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}