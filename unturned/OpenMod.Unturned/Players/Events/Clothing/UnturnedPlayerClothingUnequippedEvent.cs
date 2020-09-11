using OpenMod.Unturned.Players.Events.Equipment;

namespace OpenMod.Unturned.Players.Events.Clothing
{
    public abstract class UnturnedPlayerClothingUnequippedEvent : UnturnedPlayerUnequippedEvent
    {
        public abstract ClothingType Type { get; }

        protected UnturnedPlayerClothingUnequippedEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
