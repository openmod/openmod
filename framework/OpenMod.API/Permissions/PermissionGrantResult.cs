namespace OpenMod.API.Permissions
{
    /// <summary>
    /// Defines permission check results.
    /// </summary>
    public enum PermissionGrantResult
    {
        /// <summary>
        /// The permission was not explicitly granted or denied. Default action (which is usually the same as deny) should be performed.
        /// </summary>
        Default,

        /// <summary>
        /// The permission was explicitly granted.
        /// </summary>
        Grant,

        /// <summary>
        /// The permission was explicitly denied.
        /// </summary>
        Deny
    }
}