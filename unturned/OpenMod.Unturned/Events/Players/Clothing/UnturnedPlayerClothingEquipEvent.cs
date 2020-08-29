using OpenMod.Unturned.Entities;
using OpenMod.Unturned.Events.Players.Equipment;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players.Clothing
{
    public abstract class UnturnedPlayerClothingEquipEvent : UnturnedPlayerItemEquipEvent
    {
        protected UnturnedPlayerClothingEquipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
