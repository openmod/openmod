using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Users;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using System;

namespace OpenMod.Unturned.Events
{
    internal abstract class UnturnedEventsListener : IUnturnedEventsListener
    {
        private readonly IOpenModHost m_OpenModHost;
        private readonly IEventBus m_EventBus;
        private readonly IUserManager m_UserManager;

        protected UnturnedEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager)
        {
            m_OpenModHost = openModHost;
            m_EventBus = eventBus;
            m_UserManager = userManager;
        }

        protected UnturnedPlayer GetUnturnedPlayer(Player player)
        {
            return player == null ? null : (AsyncHelper.RunSync(() => m_UserManager.FindUserAsync(KnownActorTypes.Player,
                player.channel.owner.playerID.steamID.ToString(), UserSearchMode.FindById)) as UnturnedPlayer);
        }

        protected UnturnedPlayer GetUnturnedPlayer(SteamPlayer player)
        {
            return player == null ? null : GetUnturnedPlayer(player.player);
        }

        protected void Emit(IEvent @event)
        {
            AsyncHelper.RunSync(() => m_EventBus.EmitAsync(m_OpenModHost, this, @event));
        }

        public abstract void Subscribe();

        public abstract void Unsubscribe();
    }
}
