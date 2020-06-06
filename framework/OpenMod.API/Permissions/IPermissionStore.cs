using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMod.API.Permissions
{
    /// <summary>
    ///     The permission store is responsible for providing permissions.
    /// </summary>
    public interface IPermissionStore
    {
        /// <summary>
        ///     Gets the permissions of the given target.
        /// </summary>
        /// <param name="target">The target whichs permissions to get.</param>
        /// <param name="inherit">Defines if the parent groups permissions should be included.</param>
        /// <returns>A list of all permissions of the target.</returns>
        Task<IEnumerable<string>> GetGrantedPermissionsAsync(IPermissionActor target, bool inherit = true);

        /// <summary>
        ///     Gets the denied permissions of the given target.
        /// </summary>
        /// <param name="target">The target whichs denied permissions to get.</param>
        /// <param name="inherit">Defines if the parent groups denied permissions should be included.</param>
        /// <returns>A list of all denied permissions of the target.</returns>
        Task<IEnumerable<string>> GetDeniedPermissionsAsync(IPermissionActor target, bool inherit = true);

        /// <summary>
        ///     Adds an explicitly granted permission to the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="permission">The permission to add.</param>
        /// <returns><b>true</b> if the permission was successfully added; otherwise, <b>false</b>.</returns>
        Task<bool> AddPermissionAsync(IPermissionActor target, string permission);

        /// <summary>
        ///     Adds an explicitly denied permission to the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="permission">The denied permission to add.</param>
        /// <returns><b>true</b> if the permission was successfully added; otherwise, <b>false</b>.</returns>
        Task<bool> AddDeniedPermissionAsync(IPermissionActor target, string permission);

        /// <summary>
        ///     Removes an explicitly granted permission from the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="permission">The permission to remove.</param>
        /// <returns><b>true</b> if the permission was successfully removed; otherwise, <b>false</b>.</returns>
        Task<bool> RemovePermissionAsync(IPermissionActor target, string permission);

        /// <summary>
        ///     Removes an explicitly denied permission from the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="permission">The permission to remove.</param>
        /// <returns><b>true</b> if the permission was successfully removed; otherwise, <b>false</b>.</returns>
        Task<bool> RemoveDeniedPermissionAsync(IPermissionActor target, string permission);

        /// <summary>
        ///     Gets the primary group of the given user.
        /// </summary>
        /// <param name="user">The user wose primary group to get of.</param>
        /// <returns>the primary group if it exists; otherwise, <b>null</b>.</returns>
        [Obsolete("Might be removed")]
        Task<IPermissionGroup> GetPrimaryGroupAsync(IPermissionActor user);

        /// <summary>
        ///     Gets the primary group with the given ID.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>the group if it exists; otherwise, <b>null</b>.</returns>
        Task<IPermissionGroup> GetGroupAsync(string groupId);

        /// <summary>
        ///     Gets all inherited groups of the target. If target is a group itself, it will return the parent groups.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>the inherited groups of the target.</returns>
        Task<IEnumerable<IPermissionGroup>> GetGroupsAsync(IPermissionActor target);

        /// <summary>
        ///     Gets all registered groups.
        /// </summary>
        /// <returns>all registed groups of this provider.</returns>
        Task<IEnumerable<IPermissionGroup>> GetGroupsAsync();

        /// <summary>
        ///     Updates a group.
        /// </summary>
        /// <param name="group">The group to update.</param>
        /// <returns><b>true</b> if the group exists and could be updated; otherwise, <b>false</b>.</returns>
        Task<bool> UpdateGroupAsync(IPermissionGroup group);

        /// <summary>
        ///     Adds the given group to the user.
        /// </summary>
        /// <param name="target">The target to add the group to.</param>
        /// <param name="group">The group to add.</param>
        /// <returns><b>true</b> if the group was successfully added; otherwise, <b>false</b>.</returns>
        Task<bool> AddGroupAsync(IPermissionActor target, IPermissionGroup group);

        /// <summary>
        ///     Removes the given group from the user.
        /// </summary>
        /// <param name="target">The target to add the group to.</param>
        /// <param name="group">The group to remove.</param>
        /// <returns><b>true</b> if the group was successfully removed; otherwise, <b>false</b>.</returns>
        Task<bool> RemoveGroupAsync(IPermissionActor target, IPermissionGroup group);

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
        /// <returns><b>true</b> if the group was successfully deleted; otherwise, <b>false</b>.</returns>
        Task<bool> DeleteGroupAsync(IPermissionGroup group);

        /// <summary>
        ///     Loads the permissions from the data store.
        /// </summary>
        Task LoadAsync();

        /// <summary>
        ///     Reloads the permissions from the data store.<br /><br />
        ///     May override not saved changes.
        /// </summary>
        Task ReloadAsync();

        /// <summary>
        ///     Saves the changes to the permissions to the data store.
        /// </summary>
        Task SaveAsync();
    }
}