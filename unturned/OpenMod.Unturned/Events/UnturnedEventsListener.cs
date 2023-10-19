using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Core.Helpers;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using Steamworks;
using System;

namespace OpenMod.Unturned.Events
{
    internal abstract class UnturnedEventsListener : IUnturnedEventsListener
    {
        private readonly IOpenModHost m_OpenModHost;
        private readonly IEventBus m_EventBus;

        protected UnturnedEventsListener(IServiceProvider serviceProvider)
        {
            m_OpenModHost = serviceProvider.GetRequiredService<IOpenModHost>();
            m_EventBus = serviceProvider.GetRequiredService<IEventBus>();
        }

        protected UnturnedPlayer? GetUnturnedPlayer(Player? player)
        {
            return player == null ? null : new UnturnedPlayer(player);
        }

        protected UnturnedPlayer? GetUnturnedPlayer(SteamPlayer? player)
        {
            return GetUnturnedPlayer(player?.player);
        }

        protected UnturnedPlayer? GetUnturnedPlayer(CSteamID steamID)
        {
            return GetUnturnedPlayer(PlayerTool.getPlayer(steamID));
        }

        protected void Emit(IEvent @event)
        {
            AsyncHelper.RunSync(() => m_EventBus.EmitAsync(m_OpenModHost, this, @event));
        }

        public abstract void Subscribe();

        public abstract void Unsubscribe();
    }
}