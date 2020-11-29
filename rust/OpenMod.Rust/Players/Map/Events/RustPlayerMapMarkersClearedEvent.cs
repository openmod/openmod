using System.Collections.Generic;
using OpenMod.Rust.Players.Events;
using ProtoBuf;

namespace OpenMod.Rust.Players.Map.Events
{
    public class RustPlayerMapMarkersClearedEvent : RustPlayerEvent
    {
        public List<MapNote> MapNotes { get; }

        public RustPlayerMapMarkersClearedEvent(
            RustPlayer player,
            List<MapNote> mapNotes) : base(player)
        {
            MapNotes = mapNotes;
        }
    }
}