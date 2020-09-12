using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Crafting.Events
{
    public class UnturnedPlayerCraftingEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public UnturnedPlayerCraftingEvent(UnturnedPlayer player, ushort itemId, byte blueprintIndex) : base(player)
        {
            ItemId = itemId;
            BlueprintIndex = blueprintIndex;
        }

        public ushort ItemId { get; set; }

        public byte BlueprintIndex { get; set; }

        public bool IsCancelled { get; set; }
    }
}
