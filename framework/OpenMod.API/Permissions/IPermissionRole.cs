using System.Collections.Generic;

namespace OpenMod.API.Permissions
{
    /// <summary>
    ///     A permission role contains a collection of permissions.
    /// </summary>
    public interface IPermissionRole : IPermissionActor
    {
        /// <summary>
        ///     The permission priority of this role.
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        ///     Parents of the role
        /// </summary>
        HashSet<string> Parents { get; }

        /// <summary>
        ///    If true, this role automatically gets assigned to new users
        /// </summary>
        bool IsAutoAssigned { get; set; }
    }
}