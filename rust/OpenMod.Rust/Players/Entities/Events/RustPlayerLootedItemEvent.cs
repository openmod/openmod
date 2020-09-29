using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerLootedItemEvent : RustPlayerEvent
    {
        public PlayerLoot PlayerLoot { get; }
        public Item Item { get; }
        
        public RustPlayerLootedItemEvent(
            RustPlayer player,
            PlayerLoot playerLoot,
            Item item) : base(player)
        {
            PlayerLoot = playerLoot;
            Item = item;
        }
    }
}