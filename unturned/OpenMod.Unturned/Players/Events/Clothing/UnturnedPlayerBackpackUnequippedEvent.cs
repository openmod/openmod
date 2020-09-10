namespace OpenMod.Unturned.Players.Events.Clothing
{
    public class UnturnedPlayerBackpackUnequippedEvent : UnturnedPlayerClothingUnequippedEvent
    {
        public override ClothingType Type => ClothingType.Backpack;

        public UnturnedPlayerBackpackUnequippedEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
