using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Items;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public class PlayerItemEquipEvent : PlayerEvent, ICancellableEvent
    {
        public IItem Item { get; }

        public bool IsCancelled { get; set; }

        public PlayerItemEquipEvent(IPlayer player, IItem item) : base(player)
        {
            Item = item;
        }
    }
}