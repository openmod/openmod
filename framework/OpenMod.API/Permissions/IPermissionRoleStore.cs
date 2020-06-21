using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Permissions
{
    [Service]
    public interface IPermissionRoleStore
    {
        /// <summary>
        ///     Gets all roles inherited by the actor.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="inherit">Defines if the parent roles permissions should be included.</param>
        /// <returns>the inherited roles of the actor.</returns>
        Task<IReadOnlyCollection<IPermissionRole>> GetRolesAsync(IPermissionActor actor, bool inherit = true);

        /// <summary>
        ///     Gets all permission roles.
        /// </summary>
        /// <returns>all permission roles.</returns>
        Task<IReadOnlyCollection<IPermissionRole>> GetRolesAsync();

        /// <summary>
        ///     Gets a permission role.
        /// </summary>
        /// <returns>the permission role if found; otherwise, <b>null</b>.</returns>
        Task<IPermissionRole> GetRoleAsync(string id);

        /// <summary>
        ///     Updates a role.
        /// </summary>
        /// <param name="role">The role to update.</param>
        /// <returns><b>true</b> if the role exists and could be updated; otherwise, <b>false</b>.</returns>
        Task<bool> UpdateRoleAsync(IPermissionRole role);

        /// <summary>
        ///     Adds a role to a user.
        /// </summary>
        /// <param name="actor">The actor to add the role to.</param>
        /// <param name="roleId">The roleId to add.</param>
        /// <returns><b>true</b> if the role was successfully added or already exists; otherwise, <b>false</b>.</returns>
        Task<bool> AddRoleToActorAsync(IPermissionActor actor, string roleId);

        /// <summary>
        ///     Removes a role from a user.
        /// </summary>
        /// <param name="actor">The actor to add the role to.</param>
        /// <param name="roleId">The roleId to remove.</param>
        /// <returns><b>true</b> if the role was successfully removed or doesn't exist; otherwise, <b>false</b>.</returns>
        Task<bool> RemoveRoleFromActorAsync(IPermissionActor actor, string roleId);

        /// <summary>
        ///     Creates a new permission role.
        /// </summary>
        /// <param name="role">The role to create.</param>
        /// <returns><b>true</b> if the role was successfully created; otherwise, <b>false</b>.</returns>
        Task<bool> CreateRoleAsync(IPermissionRole role);

        /// <summary>
        ///     Deletes a permission role.
        /// </summary>
        /// <param name="roleId">The roleId to delete.</param>
        /// <returns><b>true</b> if the role was successfully deleted or doesn't exist; otherwise, <b>false</b>.</returns>
        Task<bool> DeleteRoleAsync(string roleId);

        /// <summary>
        ///     Gets the roles that will be auto assigned for the actor.
        /// </summary>
        Task<IReadOnlyCollection<string>> GetAutoAssignedRolesAsync(string actorId, string actorType);

        /// <summary>
        ///   Saves persistent data. T must be serializable.
        /// </summary>
        Task SavePersistentDataAsync<T>(string roleId, string key, T data) where T : class;

        /// <summary>
        ///   Gets persistent data. T must be serializable.
        /// </summary>
        Task<T> GetPersistentDataAsync<T>(string roleId, string key) where T : class;
    }
}