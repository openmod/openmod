using OpenMod.Unturned.Players.Events.Equipment;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public abstract class UnturnedPlayerClothingEquipEvent : UnturnedPlayerItemEquipEvent
    {
        protected UnturnedPlayerClothingEquipEvent(UnturnedPlayer player, Item item) : base(player, item)
        {

        }
    }
}
