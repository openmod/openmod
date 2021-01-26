namespace OpenMod.API.Permissions
{
    /// <summary>
    /// Represents a registered permission.
    /// </summary>
    public interface IPermissionRegistration
    {
        /// <value>
        /// The owner component.
        /// </value>
        IOpenModComponent Owner { get; }

        /// <value>
        /// The permission.
        /// </value>
        string Permission { get; }

        /// <value>
        /// The permission description. Can be null.
        /// </value>
        string? Description { get; }

        /// <value>
        /// The default grant result if the permission not explicitly granted or denied.
        /// </value>
        PermissionGrantResult DefaultGrant { get; }
    }
}