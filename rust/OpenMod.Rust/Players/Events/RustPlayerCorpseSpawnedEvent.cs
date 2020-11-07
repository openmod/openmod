namespace OpenMod.Rust.Players.Events
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