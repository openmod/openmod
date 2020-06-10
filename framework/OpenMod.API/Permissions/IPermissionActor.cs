using System.Collections.Generic;

namespace OpenMod.API.Permissions
{
    /// <summary>
    ///   Represents an actor that can be checked for permissions.
    /// </summary>
    public interface IPermissionActor
    {
        /// <summary>
        ///   The unique to the actor type and persistent ID of the actor.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///   The type of the actor.
        /// </summary>
        string Type { get; }

        /// <summary>
        ///    Persistent data for the actor
        /// </summary>
        Dictionary<string, object> Data { get; }

        /// <summary>
        ///     The human readable name
        /// </summary>
        string DisplayName { get; }
    }
}