using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Events.Crafting
{
    public class UnturnedPlayerCraftEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public UnturnedPlayerCraftEvent(UnturnedPlayer player, ushort itemId, byte blueprintIndex) : base(player)
        {
            ItemId = itemId;
            BlueprintIndex = blueprintIndex;
        }

        public ushort ItemId { get; set; }

        public byte BlueprintIndex { get; set; }

        public bool IsCancelled { get; set; }
    }
}
