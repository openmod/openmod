using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerGlassesEquippedEvent : UnturnedPlayerClothingEquippedEvent
    {
        public override ClothingType Type => ClothingType.Glasses;

        public UnturnedPlayerGlassesEquippedEvent(UnturnedPlayer player, UnturnedItem item) : base(player, item)
        {

        }
    }
}
