using OpenMod.Rust.Players.Events;
using ProtoBuf;

namespace OpenMod.Rust.Players.Map.Events
{
    public class RustPlayerMapMarkerAddedEvent : RustPlayerEvent
    {
        public MapNote MapNote { get; }

        public RustPlayerMapMarkerAddedEvent(
            RustPlayer player,
            MapNote mapNote) : base(player)
        {
            MapNote = mapNote;
        }
    }
}