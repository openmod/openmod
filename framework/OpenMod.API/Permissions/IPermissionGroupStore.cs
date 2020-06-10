using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Permissions
{
    [Service]
    public interface IPermissionGroupStore
    {
        /// <summary>
        ///     Gets all groups inherited by the actor.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <returns>the inherited groups of the actor.</returns>
        Task<IReadOnlyCollection<IPermissionGroup>> GetGroupsAsync(IPermissionActor actor);

        /// <summary>
        ///     Gets all permission groups.
        /// </summary>
        /// <returns>all permission groups.</returns>
        Task<IReadOnlyCollection<IPermissionGroup>> GetGroupsAsync();

        /// <summary>
        ///     Gets a permission group.
        /// </summary>
        /// <returns>the permission group if found; otherwise, <b>null</b>.</returns>
        Task<IPermissionGroup> GetGroupAsync(string id);

        /// <summary>
        ///     Updates a group.
        /// </summary>
        /// <param name="group">The group to update.</param>
        /// <returns><b>true</b> if the group exists and could be updated; otherwise, <b>false</b>.</returns>
        Task<bool> UpdateGroupAsync(IPermissionGroup group);

        /// <summary>
        ///     Adds a group to a user.
        /// </summary>
        /// <param name="actor">The actor to add the group to.</param>
        /// <param name="group">The group to add.</param>
        /// <returns><b>true</b> if the group was successfully added or already exists; otherwise, <b>false</b>.</returns>
        Task<bool> AddGroupAsync(IPermissionActor actor, IPermissionGroup group);

        /// <summary>
        ///     Removes a group from a user.
        /// </summary>
        /// <param name="actor">The actor to add the group to.</param>
        /// <param name="group">The group to remove.</param>
        /// <returns><b>true</b> if the group was successfully removed or doesn't exist; otherwise, <b>false</b>.</returns>
        Task<bool> RemoveGroupAsync(IPermissionActor actor, IPermissionGroup group);

        /// <summary>
        ///     Creates a new permission group.
        /// </summary>
        /// <param name="group">The group to create.</param>
        /// <returns><b>true</b> if the group was successfully created; otherwise, <b>false</b>.</returns>
        Task<bool> CreateGroupAsync(IPermissionGroup group);

        /// <summary>
        ///     Deletes a permission group.
        /// </summary>
        /// <param name="group">The group to delete.</param>
        /// <returns><b>true</b> if the group was successfully deleted or doesn't exist; otherwise, <b>false</b>.</returns>
        Task<bool> DeleteGroupAsync(IPermissionGroup group);

        /// <summary>
        ///     Assigns the user to all permission groups that have IsAutoAssigned set to true.
        /// </summary>
        /// <param name="actor">The user to assign</param>
        Task AssignAutoGroupsToUserAsync(IPermissionActor actor);
    }
}