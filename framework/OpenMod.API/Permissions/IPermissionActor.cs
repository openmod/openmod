namespace OpenMod.API.Permissions
{
    /// <summary>
    /// Represents an actor that can have permissions.
    /// </summary>
    public interface IPermissionActor
    {
        /// <summary>
        /// The unique and persistent ID of the actor.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The type of the actor.
        /// </summary>
        string Type { get; }
    }
}