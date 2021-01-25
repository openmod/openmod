using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMod.API.Permissions
{
    /// <summary>
    /// Represents a permission role.
    /// </summary>
    public interface IPermissionRole : IPermissionActor
    {
        /// <value>
        /// The priority of this role.
        /// </value>
        int Priority { get; set; }

        /// <value>
        /// Gets the parents of the role. Can be null but items cannot be null.
        /// </value>
        [ItemNotNull]
        [CanBeNull]
        HashSet<string> Parents { get; }

        /// <value>
        /// True if this role should automatically get assigned to new users.
        /// </value>
        bool IsAutoAssigned { get; set; }
    }
}