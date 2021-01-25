namespace OpenMod.API.Permissions
{
    /// <summary>
    /// Represents an actor that can be checked for permissions.
    /// </summary>
    public interface IPermissionActor
    {
        /// <value>
        /// The unique to the actor type and persistent ID of the actor.
        /// </value>
        string Id { get; }

        /// <value>
        /// The type of the actor.
        /// </value>
        string Type { get; }

        /// <value>
        /// The human readable name of the actor.
        /// </value>
        string DisplayName { get; }
    }
}