namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerHatUnequippedEvent : UnturnedPlayerClothingUnequippedEvent
    {
        public override ClothingType Type => ClothingType.Hat;

        public UnturnedPlayerHatUnequippedEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
