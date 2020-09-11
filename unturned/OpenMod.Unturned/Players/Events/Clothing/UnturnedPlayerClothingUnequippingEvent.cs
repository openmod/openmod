using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players.Events.Equipment;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public abstract class UnturnedPlayerClothingUnequippingEvent : UnturnedPlayerUnequippingEvent
    {
        public abstract ClothingType Type { get; }

        protected UnturnedPlayerClothingUnequippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
