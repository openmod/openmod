using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players.Clothing
{
    public class UnturnedPlayerShirtDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public UnturnedPlayerShirtDequipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
