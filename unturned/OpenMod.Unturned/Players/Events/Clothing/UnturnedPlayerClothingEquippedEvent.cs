using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players.Events.Equipment;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerClothingEquippedEvent : UnturnedPlayerItemEquippedEvent
    {
        public ClothingType Type { get; }

        public UnturnedPlayerClothingEquippedEvent(UnturnedPlayer player, UnturnedItem item, ClothingType type) : base(player, item)
        {
            Type = type;
        }
    }
}
