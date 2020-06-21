using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMod.API.Permissions
{
    public interface IPermissionStore
    {
        /// <summary>
        ///     Gets the permissions of the given actor.
        /// </summary>
        /// <param name="actor">The actor to get permissions from.</param>
        /// <param name="inherit">Defines if the parent roles permissions should be included.</param>
        /// <returns>A list of all permissions of the actor.</returns>
        Task<IReadOnlyCollection<string>> GetGrantedPermissionsAsync(IPermissionActor actor, bool inherit = true);

        /// <summary>
        ///     Gets the denied permissions of the given actor.
        /// </summary>
        /// <param name="actor">The actor to get denied permissions from.</param>
        /// <param name="inherit">Defines if the parent roles denied permissions should be included.</param>
        /// <returns>A list of all denied permissions of the actor.</returns>
        Task<IReadOnlyCollection<string>> GetDeniedPermissionsAsync(IPermissionActor actor, bool inherit = true);

        /// <summary>
        ///     Adds an explicitly granted permission to the actor.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="permission">The permission to add.</param>
        /// <returns><b>true</b> if the permission was successfully added or exists already; otherwise, <b>false</b>.</returns>
        Task<bool> AddGrantedPermissionAsync(IPermissionActor actor, string permission);

        /// <summary>
        ///     Adds an explicitly denied permission to the actor.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="permission">The denied permission to add.</param>
        /// <returns><b>true</b> if the permission was successfully added or exists already; otherwise, <b>false</b>.</returns>
        Task<bool> AddDeniedPermissionAsync(IPermissionActor actor, string permission);

        /// <summary>
        ///     Removes an explicitly granted permission from the actor.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="permission">The permission to remove.</param>
        /// <returns><b>true</b> if the permission was successfully removed or doesn't exist; otherwise, <b>false</b>.</returns>
        Task<bool> RemoveGrantedPermissionAsync(IPermissionActor actor, string permission);

        /// <summary>
        ///     Removes an explicitly denied permission from the actor.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="permission">The permission to remove.</param>
        /// <returns><b>true</b> if the permission was successfully removed or doesn't exist; otherwise, <b>false</b>.</returns>
        Task<bool> RemoveDeniedPermissionAsync(IPermissionActor actor, string permission);
    }
}