using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Users;
using OpenMod.Core.Users.Events;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Users
{
    [Priority(Priority = Priority.Low)]
    public class UnturnedUserProvider : IUserProvider, IDisposable
    {
        private readonly HashSet<UnturnedUser> m_UnturnedUsers;
        private readonly HashSet<UnturnedPendingUser> m_PendingUsers;

        private readonly IEventBus m_EventBus;
        private readonly IRuntime m_Runtime;
        private readonly IUserDataSeeder m_DataSeeder;
        private readonly IUserDataStore m_UserDataStore;

        public UnturnedUserProvider(
            IEventBus eventBus, 
            IUserDataSeeder dataSeeder, 
            IUserDataStore userDataStore, 
            IRuntime runtime)
        {
            m_EventBus = eventBus;
            m_DataSeeder = dataSeeder;
            m_UserDataStore = userDataStore;
            m_Runtime = runtime;
            m_UnturnedUsers = new HashSet<UnturnedUser>();
            m_PendingUsers = new HashSet<UnturnedPendingUser>();

            Provider.onCheckValidWithExplanation += OnPendingPlayerConnected;
            Provider.onEnemyConnected += OnPlayerConnected;
            Provider.onEnemyDisconnected += OnEnemyDisconnected;
        }

        protected virtual void OnEnemyDisconnected(SteamPlayer player)
        {
            AsyncHelper.RunSync(async () =>
            {
                var user = GetUser(player);
                if (user.Session is UnturnedUserSession session)
                {
                    session.OnSessionEnd();
                }

                var disconnectedEvent = new UserDisconnectedEvent(user);

                await m_EventBus.EmitAsync(m_Runtime, this, disconnectedEvent);
                m_UnturnedUsers.Remove(user);

                var userData = await m_UserDataStore.GetUserDataAsync(user.Id, user.Type);
                userData.LastSeen = DateTime.Now;
                await m_UserDataStore.SaveUserDataAsync(userData);
            });
        }

        public virtual UnturnedUser GetUser(Player player)
        {
            return GetUser(player.channel.owner);
        }

        public virtual UnturnedUser GetUser(SteamPlayer player)
        {
            return GetUser(player.playerID.steamID);
        }

        public virtual UnturnedUser GetUser(CSteamID id)
        {
            return m_UnturnedUsers.FirstOrDefault(d => d.SteamId == id);
        }

        protected virtual void OnPlayerConnected(SteamPlayer player)
        {
            AsyncHelper.RunSync(async () =>
            {
                var pending = m_PendingUsers.First(d => d.SteamId == player.playerID.steamID);
                FinishSession(pending);

                var user = new UnturnedUser(m_UserDataStore, player.player, pending);
                m_UnturnedUsers.Add(user);

                var connectedEvent = new UserConnectedEvent(user);
                await m_EventBus.EmitAsync(m_Runtime, this, connectedEvent);
            });
        }

        // todo: memory leak, m_PendingUsers does not get cleared up when pending user gets rejected
        // pending player session also does not end on rejections.
        // Unturned does not have an event for handling rejections yet.
        protected virtual void OnPendingPlayerConnected(ValidateAuthTicketResponse_t callback, ref bool isvalid, ref string explanation)
        {
            var isvalid_ = isvalid;
            var explanation_ = explanation;

            AsyncHelper.RunSync(async () =>
            {
                if (!isvalid_)
                {
                    return;
                }

                var steamPending = Provider.pending.First(d => d.playerID.steamID == callback.m_SteamID);
                var pendingUser = new UnturnedPendingUser(m_UserDataStore, steamPending);
                await m_DataSeeder.SeedUserDataAsync(pendingUser.Id, pendingUser.Type, pendingUser.DisplayName);

                var userData = await m_UserDataStore.GetUserDataAsync(pendingUser.Type, pendingUser.Id);
                userData.LastSeen = DateTime.Now;
                userData.LastDisplayName = pendingUser.DisplayName;
                await m_UserDataStore.SaveUserDataAsync(userData);

                m_PendingUsers.Add(pendingUser);

                var userConnectingEvent = new UserConnectingEvent(pendingUser);
                await m_EventBus.EmitAsync(m_Runtime, this, userConnectingEvent);

                if (!string.IsNullOrEmpty(userConnectingEvent.RejectionReason))
                {
                    isvalid_ = false;
                    explanation_ = userConnectingEvent.RejectionReason;
                }

                if (userConnectingEvent.IsCancelled)
                {
                    isvalid_ = false;
                }
            });

            isvalid = isvalid_;
            explanation = explanation_;
        }

        protected virtual void FinishSession(UnturnedPendingUser pending)
        {
            if (pending.Session is UnturnedPendingUserSession session)
            {
                session.OnSessionEnd();
            }

            m_PendingUsers.Remove(pending);
        }

        public bool SupportsUserType(string userType)
        {
            return userType.Equals(KnownActorTypes.Player, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IUser> FindUserAsync(string userType, string searchString, UserSearchMode searchMode)
        {
            switch (searchMode)
            {
                case UserSearchMode.Id:
                    return m_UnturnedUsers.FirstOrDefault(d => d.Id.Equals(searchString, StringComparison.OrdinalIgnoreCase));
                case UserSearchMode.Name:
                    return m_UnturnedUsers.FirstOrDefault(d => d.DisplayName.Equals(searchString, StringComparison.OrdinalIgnoreCase))
                        ?? m_UnturnedUsers.FirstOrDefault(d => d.DisplayName.StartsWith(searchString, StringComparison.OrdinalIgnoreCase));
                case UserSearchMode.NameOrId:
                    return await FindUserAsync(userType, searchString, UserSearchMode.Id) ??
                           await FindUserAsync(userType, searchString, UserSearchMode.Name);
                default:
                    throw new ArgumentOutOfRangeException(nameof(searchMode), searchMode, null);
            }
        }

        public Task<IReadOnlyCollection<IUser>> GetUsers(string userType)
        {
            return Task.FromResult<IReadOnlyCollection<IUser>>(m_UnturnedUsers);
        }

        public void Dispose()
        {
            Provider.onCheckValidWithExplanation -= OnPendingPlayerConnected;
            Provider.onEnemyConnected -= OnPlayerConnected;
            Provider.onEnemyDisconnected -= OnEnemyDisconnected;
        }
    }
}