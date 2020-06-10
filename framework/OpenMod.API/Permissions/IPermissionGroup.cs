using System.Collections.Generic;

namespace OpenMod.API.Permissions
{
    /// <summary>
    ///     A permission group contains a collection of permissions.
    /// </summary>
    public interface IPermissionGroup : IPermissionActor
    {
        /// <summary>
        ///     The permission priority of this group.
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        ///     Parents of the group
        /// </summary>
        ICollection<string> Parents { get; }

        /// <summary>
        ///    If true, this group automatically gets assigned to new users
        /// </summary>
        bool IsAutoAssigned { get; set; }
    }
}