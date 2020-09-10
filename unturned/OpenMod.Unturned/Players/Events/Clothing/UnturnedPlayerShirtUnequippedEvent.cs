using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerShirtUnequippedEvent : UnturnedPlayerClothingUnequippedEvent
    {
        public override ClothingType Type => ClothingType.Shirt;

        public UnturnedPlayerShirtUnequippedEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
