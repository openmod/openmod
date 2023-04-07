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
            SDG.Unturned.Provider.onServerHosted += OnServerHosted;
        }

        public override void Unsubscribe()
        {
            SDG.Unturned.Provider.onCommenceShutdown -= OnCommenceShutdown;
            SDG.Unturned.Provider.onServerHosted -= OnServerHosted;
        }

        private void OnCommenceShutdown()
        {
            var @event = new UnturnedShutdownCommencedEvent();

            Emit(@event);
        }

        private void OnServerHosted()
        {
            var @event = new UnturnedServerHostedEvent();

            Emit(@event);
        }
    }
}
