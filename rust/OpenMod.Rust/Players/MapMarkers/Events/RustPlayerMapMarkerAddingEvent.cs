using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;
using ProtoBuf;

namespace OpenMod.Rust.Players.MapMarkers.Events
{
    public class RustPlayerMapMarkerAddingEvent : RustPlayerEvent, ICancellableEvent
    {
        public MapNote MapNote { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerMapMarkerAddingEvent(
            RustPlayer player,
            MapNote mapNote) : base(player)
        {
            MapNote = mapNote;
        }
    }
}