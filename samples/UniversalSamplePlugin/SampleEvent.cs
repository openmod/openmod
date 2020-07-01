using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;

namespace UniversalSamplePlugin
{
    public class SampleEvent : Event, ICancellableEvent
    {
        public int Value { get; set; }
        public bool IsCancelled { get; set; }
    }
}