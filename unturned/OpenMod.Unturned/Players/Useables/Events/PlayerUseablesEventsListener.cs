using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Useables.Events
{
    internal class PlayerUseablesEventsListener : UnturnedEventsListener
    {
        public PlayerUseablesEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {
        }

        public override void Subscribe()
        {
            UseableConsumeable.onPerformingAid += OnPerformingAid;
        }

        public override void Unsubscribe()
        {
            UseableConsumeable.onPerformingAid -= OnPerformingAid;
        }

        private void OnPerformingAid(Player nativeInstigator, Player nativeTarget, ItemConsumeableAsset asset, ref bool shouldAllow)
        {
            var instigator = GetUnturnedPlayer(nativeInstigator);
            var target = GetUnturnedPlayer(nativeTarget);

            var @event = new UnturnedPlayerPerformingAidEvent(instigator!, target!, asset)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }
    }
}
