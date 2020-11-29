using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerCorpseSpawnedEvent : RustPlayerEvent
    {
        public BaseCorpse Corpse { get; }

        public RustPlayerCorpseSpawnedEvent(
            RustPlayer player,
            BaseCorpse corpse) : base(player)
        {
            Corpse = corpse;
        }
    }
}