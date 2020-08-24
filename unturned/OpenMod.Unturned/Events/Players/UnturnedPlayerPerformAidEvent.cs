using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players
{
    public class UnturnedPlayerPerformAidEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public UnturnedPlayer Target { get; }

        public ItemConsumeableAsset ConsumeableAsset { get; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerPerformAidEvent(UnturnedPlayer player, UnturnedPlayer target, ItemConsumeableAsset consumeableAsset) : base(player)
        {
            Target = target;
            ConsumeableAsset = consumeableAsset;
        }
    }
}
