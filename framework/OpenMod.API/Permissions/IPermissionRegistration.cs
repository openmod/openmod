using JetBrains.Annotations;

namespace OpenMod.API.Permissions
{
    /// <summary>
    /// Represents a registered permission.
    /// </summary>
    public interface IPermissionRegistration
    {
        /// <value>
        /// The owner component. Cannot be null.
        /// </value>
        [NotNull]
        IOpenModComponent Owner { get; }

        /// <value>
        /// The permission. Cannot be null or empty.
        /// </value>
        [NotNull]
        string Permission { get; }

        /// <value>
        /// The permission description. Can be null.
        /// </value>
        [CanBeNull]
        string Description { get; }

        /// <value>
        /// The default grant result if the permission not explicitly granted or denied.
        /// </value>
        PermissionGrantResult DefaultGrant { get; }
    }
}