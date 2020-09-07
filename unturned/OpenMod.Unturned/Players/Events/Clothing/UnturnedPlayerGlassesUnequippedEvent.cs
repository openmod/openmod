namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerGlassesUnequippedEvent : UnturnedPlayerClothingUnequippedEvent
    {
        public override ClothingType Type => ClothingType.Glasses;

        public UnturnedPlayerGlassesUnequippedEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
