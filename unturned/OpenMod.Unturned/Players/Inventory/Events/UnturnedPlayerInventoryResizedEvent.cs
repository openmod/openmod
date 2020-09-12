using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Inventory.Events
{
    public class UnturnedPlayerInventoryResizedEvent : UnturnedPlayerEvent
    {
        public byte Page { get; }

        public byte Width { get; }

        public byte Height { get; }

        public UnturnedPlayerInventoryResizedEvent(UnturnedPlayer player, byte page, byte width, byte height) : base(player)
        {
            Page = page;
            Width = width;
            Height = height;
        }
    }
}
