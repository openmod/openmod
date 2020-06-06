namespace OpenMod.API.Permissions
{
    /// <summary>
    ///     Result of a permission check.
    /// </summary>
    public enum PermissionStatus
    {
        /// <summary>
        ///     The permission was neither denied nor granted. Default action (which is usually the same as deny) should be
        ///     executed.
        /// </summary>
        Default,

        /// <summary>
        ///     The permission was explicitly granted.
        /// </summary>
        Grant,

        /// <summary>
        ///     The permission was explicitly denied.
        /// </summary>
        Deny
    }
}