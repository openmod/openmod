using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using System;

namespace OpenMod.Unturned.Events
{
    internal abstract class UnturnedEventsListener : IUnturnedEventsListener
    {
        private readonly IOpenModHost m_OpenModHost;
        private readonly IEventBus m_EventBus;
        protected readonly IUserManager UserManager;

        protected UnturnedEventsListener(IServiceProvider serviceProvider)
        {
            m_OpenModHost = serviceProvider.GetRequiredService<IOpenModHost>();
            m_EventBus = serviceProvider.GetRequiredService<IEventBus>();
            UserManager = serviceProvider.GetRequiredService<IUserManager>();
        }

        protected UnturnedPlayer? GetUnturnedPlayer(Player? player)
        {
            return player == null ? null : new UnturnedPlayer(player);
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
