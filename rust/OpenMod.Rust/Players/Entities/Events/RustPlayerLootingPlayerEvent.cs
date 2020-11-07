namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerLootingPlayerEvent : RustPlayerLootingEntityEvent
    {
        public RustPlayer Target { get; }

        public RustPlayerLootingPlayerEvent(
            RustPlayer player,
            RustPlayer target) : base(player, target)
        {
            Target = target;
        }
    }
}