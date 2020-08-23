using OpenMod.API;
using OpenMod.Core.Eventing;

namespace OpenMod.Runtime.Events
{
    public sealed class OpenModInitializedEvent : Event
    {
        public IOpenModHost Host { get; }

        public OpenModInitializedEvent(IOpenModHost host)
        {
            Host = host;
        }
    }
}