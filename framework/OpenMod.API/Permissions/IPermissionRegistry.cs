using System.Collections.Generic;
using OpenMod.API.Ioc;

namespace OpenMod.API.Permissions
{
    /// <summary>
    /// The service used for registering and looking up permissions.
    /// </summary>
    [Service]
    public interface IPermissionRegistry
    {
        /// <summary>
        /// Registers a permission.
        /// </summary>
        /// <param name="component">The component registering the permission.</param>
        /// <param name="permission">The permission.</param>
        /// <param name="description">The optional description.</param>
        /// <param name="defaultGrant">The optional default grant. Will default to <see cref="PermissionGrantResult.Default"/>. See <see cref="IPermissionRegistration.DefaultGrant"/>.</param>
        void RegisterPermission(
            IOpenModComponent component, 
            string permission, 
            string? description = null, 
            PermissionGrantResult? defaultGrant = null);

        /// <summary>
        /// Gets the registered permissions for the given component.
        /// </summary>
        /// <param name="component">The component to get the permissions of.</param>
        /// <returns>The registered permissions of the given component.</returns>
        IReadOnlyCollection<IPermissionRegistration> GetPermissions(IOpenModComponent component);

        /// <summary>
        /// Searches for a permission registration.
        /// </summary>
        /// <param name="permission">The permission to search for.</param>
        /// <returns><b>The registered permission</b> if founds; otherwise, <b>null</b>.</returns>
        IPermissionRegistration? FindPermission(string permission);

        /// <summary>
        /// Searches for a permission registration.
        /// </summary>
        /// <param name="component">The component that has registered the permission.</param>
        /// <param name="permission">The permission to search for.</param>
        /// <returns><b>The registered permission</b> if founds; otherwise, <b>null</b>.</returns>
        IPermissionRegistration? FindPermission(IOpenModComponent component, string permission);
    }
}