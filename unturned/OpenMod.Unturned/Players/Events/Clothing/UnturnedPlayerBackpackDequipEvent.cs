using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerBackpackDequipEvent : UnturnedPlayerClothingDequipEvent
    {
        public UnturnedPlayerBackpackDequipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
