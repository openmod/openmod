using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Provider.Events
{
    class ProviderEventsListener : UnturnedEventsListener
    {
        public ProviderEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {
        }

        public override void Subscribe()
        {
            SDG.Unturned.Provider.onCommenceShutdown += OnCommenceShutdown;
        }

        public override void Unsubscribe()
        {
            SDG.Unturned.Provider.onCommenceShutdown -= OnCommenceShutdown;
        }

        private void OnCommenceShutdown()
        {
            var @event = new UnturnedShutdownCommencedEvent();

            Emit(@event);
        }
    }
}
