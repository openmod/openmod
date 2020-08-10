using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Items;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public class PlayerItemUnequipEvent : PlayerEvent, ICancellableEvent
    {
        public IItem Item { get; }

        public bool IsCancelled { get; set; }

        public PlayerItemUnequipEvent(IPlayer player, IItem item) : base(player)
        {
            Item = item;
        }
    }
}