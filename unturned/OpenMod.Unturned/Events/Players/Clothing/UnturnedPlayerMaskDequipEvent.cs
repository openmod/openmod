using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players.Clothing
{
    public class UnturnedPlayerMaskDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public UnturnedPlayerMaskDequipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
