using System.Collections.Generic;

namespace OpenMod.API.Eventing
{
    /// <summary>
    ///     Base representation of an event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        ///     The name of the event.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The arguments of the event.
        /// </summary>
        Dictionary<string, object> Arguments { get; }

        /// <summary>
        ///    Event data
        /// </summary>
        Dictionary<string, object> Data { get; }
    }
}