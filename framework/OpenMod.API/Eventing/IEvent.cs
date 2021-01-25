using System.Collections.Generic;

namespace OpenMod.API.Eventing
{
    /// <summary>
    /// Represents a event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// The arguments of the event. Based on the properties of the event object.
        /// </summary>
        Dictionary<string, object> Arguments { get; }

        /// <summary>
        /// Arbitrary additonal event data that can be set and used by plugins. 
        /// </summary>
        Dictionary<string, object> Data { get; }
    }
}