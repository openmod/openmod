using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;
using ProtoBuf;

namespace OpenMod.Rust.Players.Map.Events
{
    public class RustPlayerMapMarkerRemovingEvent : RustPlayerEvent, ICancellableEvent
    {
        public MapNote MapNote { get; }
        public bool IsCancelled { get; set; }
     
        public RustPlayerMapMarkerRemovingEvent(
            RustPlayer player,
            MapNote mapNote) : base(player)
        {
            MapNote = mapNote;
        }
    }
}