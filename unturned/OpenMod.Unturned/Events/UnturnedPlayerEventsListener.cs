using SDG.Unturned;
using System;

namespace OpenMod.Unturned.Events
{
    internal abstract class UnturnedPlayerEventsListener : UnturnedEventsListener, IUnturnedPlayerEventsListener
    {
        protected UnturnedPlayerEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public abstract void SubscribePlayer(Player player);

        public abstract void UnsubscribePlayer(Player player);
    }
}
