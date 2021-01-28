using System.Collections.Generic;

namespace OpenMod.API.Permissions
{
    /// <summary>
    /// Represents a permission role.
    /// </summary>
    public interface IPermissionRole : IPermissionActor
    {
        /// <summary>
        /// Gets or sets the priority of this role.
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        /// Gets or sets the parents of the role.
        /// </summary>
        HashSet<string> Parents { get; }

        /// <summary>
        /// Defines if the role should be automatically assigned to new users.
        /// </summary>
        bool IsAutoAssigned { get; set; }
    }
}