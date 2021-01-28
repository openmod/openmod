namespace OpenMod.API.Permissions
{
    /// <summary>
    /// Represents a registered permission.
    /// </summary>
    public interface IPermissionRegistration
    {
        /// <summary>
        /// Gets the owner component.
        /// </summary>
        IOpenModComponent Owner { get; }

        /// <summary>
        /// Gets the permission.
        /// </summary>
        string Permission { get; }

        /// <summary>
        /// Gets the permission description.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets the default grant result if the permission not explicitly granted or denied.
        /// </summary>
        PermissionGrantResult DefaultGrant { get; }
    }
}