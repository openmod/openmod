using System;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Unturned.Players;
using SDG.Unturned;

namespace OpenMod.Unturned.Events
{
    internal abstract class UnturnedEventsListener : IUnturnedEventsListener
    {
        private readonly IOpenModHost m_OpenModHost;
        private readonly IEventBus m_EventBus;
        protected readonly IUserManager m_UserManager;

        protected UnturnedEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager)
        {
            m_OpenModHost = openModHost;
            m_EventBus = eventBus;
            m_UserManager = userManager;
        }

        protected UnturnedPlayer? GetUnturnedPlayer(Player? player)
        {
            if (player == null)
            {
                return null;
            }

            return new UnturnedPlayer(player);
        }

        protected UnturnedPlayer? GetUnturnedPlayer(SteamPlayer? player)
        {
            return GetUnturnedPlayer(player?.player);
        }

        protected void Emit(IEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            AsyncHelper.RunSync(() => m_EventBus.EmitAsync(m_OpenModHost, this, @event));
        }

        public abstract void Subscribe();

        public abstract void Unsubscribe();
    }
}
