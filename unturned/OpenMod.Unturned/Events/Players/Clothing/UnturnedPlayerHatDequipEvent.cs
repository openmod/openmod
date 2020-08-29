using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players.Clothing
{
    public class UnturnedPlayerHatDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public UnturnedPlayerHatDequipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
