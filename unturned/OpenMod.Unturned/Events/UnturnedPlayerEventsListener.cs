using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using SDG.Unturned;

namespace OpenMod.Unturned.Events
{
    internal abstract class UnturnedPlayerEventsListener : UnturnedEventsListener, IUnturnedPlayerEventsListener
    {
        protected UnturnedPlayerEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

        }

        public abstract void SubscribePlayer(Player player);

        public abstract void UnsubscribePlayer(Player player);
    }
}
