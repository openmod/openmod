using System.Threading.Tasks;

namespace OpenMod.API.Permissions
{
    public interface IPermissionChecker
    {
        /// <summary>
        ///     Defines if the given target is supported by this provider.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <returns><b>true</b> if the given target is supported; otherwise, <b>false</b>.</returns>
        bool SupportsTarget(IPermissionActor target);

        /// <summary>
        ///     Check if the target has the given permission.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <param name="permission">The permission to check.</param>
        /// <returns>
        ///     <see cref="PermissionResult.Grant" /> if the target explicity has the permission,
        ///     <see cref="PermissionResult.Deny" /> if the target explicitly does not have the permission; otherwise,
        ///     <see cref="PermissionResult.Default" />
        /// </returns>
        Task<PermissionStatus> CheckPermissionAsync(IPermissionActor target, string permission);
    }
}