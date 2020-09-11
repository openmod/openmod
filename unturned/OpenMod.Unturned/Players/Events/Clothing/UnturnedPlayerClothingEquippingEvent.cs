using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players.Events.Equipment;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public abstract class UnturnedPlayerClothingEquippingEvent : UnturnedPlayerEquippingEvent
    {
        public abstract ClothingType Type { get; }

        protected UnturnedPlayerClothingEquippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
