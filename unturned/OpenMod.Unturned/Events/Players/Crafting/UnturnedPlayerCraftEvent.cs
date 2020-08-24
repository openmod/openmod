using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Crafting
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
