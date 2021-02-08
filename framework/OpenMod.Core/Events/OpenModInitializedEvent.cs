using OpenMod.API;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Events
{
    /// <summary>
    /// The event that is triggered when OpenMod has been initialized. 
    /// It is called after the container has been built and all plugins have been loaded.
    /// </summary>
    public sealed class OpenModInitializedEvent : Event
    {
        /// <summary>
        /// The OpenMod host.
        /// </summary>
        public IOpenModHost Host { get; }

        public OpenModInitializedEvent(IOpenModHost host)
        {
            Host = host;
        }
    }
}