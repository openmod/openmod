using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players.Equipment
{
    public class UnturnedPlayerItemEquipEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public ItemJar ItemJar { get; }

        public ItemAsset ItemAsset { get; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerItemEquipEvent(UnturnedPlayer player, ItemJar itemJar, ItemAsset itemAsset) : base(player)
        {
            ItemJar = itemJar;
            ItemAsset = itemAsset;
        }
    }
}
