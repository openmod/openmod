using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Useables
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
            UnturnedPlayer instigator = GetUnturnedPlayer(nativeInstigator);
            UnturnedPlayer target = GetUnturnedPlayer(nativeTarget);

            UnturnedPlayerPerformAidEvent @event = new UnturnedPlayerPerformAidEvent(instigator, target, asset);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }
    }
}
