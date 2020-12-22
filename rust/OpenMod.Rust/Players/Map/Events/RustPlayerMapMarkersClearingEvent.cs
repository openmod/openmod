using System.Collections.Generic;
using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;
using ProtoBuf;

namespace OpenMod.Rust.Players.Map.Events
{
    public class RustPlayerMapMarkersClearingEvent : RustPlayerEvent, ICancellableEvent
    {
        public List<MapNote> MapNotes { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerMapMarkersClearingEvent(
            RustPlayer player,
            List<MapNote> mapNotes) : base(player)
        {
            MapNotes = mapNotes;
        }
    }
}