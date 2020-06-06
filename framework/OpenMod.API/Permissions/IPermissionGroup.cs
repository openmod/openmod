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
        int Priority { get; }

        /// <summary>
        ///     The human readable name of the permission group
        /// </summary>
        string DisplayName { get; }
    }
}