using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players.Events.Equipment;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public abstract class UnturnedPlayerClothingEquippedEvent : UnturnedPlayerEquippedEvent
    {
        public abstract ClothingType Type { get; }

        protected UnturnedPlayerClothingEquippedEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
