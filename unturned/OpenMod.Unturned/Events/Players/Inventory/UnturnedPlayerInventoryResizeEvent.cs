using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Inventory
{
    public class UnturnedPlayerInventoryResizeEvent : UnturnedPlayerEvent
    {
        public byte Page { get; }

        public byte Width { get; }

        public byte Height { get; }

        public UnturnedPlayerInventoryResizeEvent(UnturnedPlayer player, byte page, byte width, byte height) : base(player)
        {
            Page = page;
            Width = width;
            Height = height;
        }
    }
}
