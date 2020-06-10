using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Permissions
{
    /// <summary>
    ///     Checks if an actor is authorized to do an action.
    /// </summary>
    [Service]
    public interface IPermissionChecker
    {
        /// <summary>
        ///    Permission check providers.
        /// </summary>
        IReadOnlyCollection<IPermissionCheckProvider> PermissionCheckProviders { get; }

        /// <summary>
        ///   Permission sources.
        /// </summary>
        IReadOnlyCollection<IPermissionStore> PermissionStores { get; }

        /// <summary>
        ///     Checks if an actor has permission to execute an action.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="permission">The permission to check.</param>
        /// <returns></returns>
        Task<PermissionGrantResult> CheckPermissionAsync(IPermissionActor actor, string permission);

        /// <summary>
        ///     Initializes the permission checker.
        /// </summary>
        Task InitAsync();
    }
}