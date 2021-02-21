namespace OpenMod.API.Permissions
{
    /// <summary>
    /// Represents an actor that can be checked for permissions.
    /// </summary>
    public interface IPermissionActor
    {
        /// <summary>
        /// Gets the unique to the actor type and persistent ID of the actor.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the type of the actor.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the human readable name of the actor.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the full actor name including display name and ID
        /// </summary>
        string FullActorName { get; }
    }
}