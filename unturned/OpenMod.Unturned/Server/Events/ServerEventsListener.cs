using OpenMod.API;
using OpenMod.Unturned.Events;
using System;

namespace OpenMod.Unturned.Server.Events
{
    [OpenModInternal]
    internal class ServerEventsListener : UnturnedEventsListener
    {
        public ServerEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
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
