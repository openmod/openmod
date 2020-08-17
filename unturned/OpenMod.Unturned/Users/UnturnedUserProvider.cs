using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Users;
using OpenMod.Core.Users.Events;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace OpenMod.Unturned.Users
{
    [Priority(Priority = Priority.Low)]
    public class UnturnedUserProvider : IUserProvider, IDisposable
    {
        private readonly HashSet<UnturnedPlayer> m_Players;
        private readonly HashSet<UnturnedPendingPlayer> m_PendingPlayers;

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
            m_Players = new HashSet<UnturnedPlayer>();
            m_PendingPlayers = new HashSet<UnturnedPendingPlayer>();

            // sync users in case of openmod reloads
            foreach (var client in Provider.clients)
            {
                m_Players.Add(new UnturnedPlayer(this, userDataStore, client.player));
            }

            foreach (var pending in Provider.pending)
            {
                m_PendingPlayers.Add(new UnturnedPendingPlayer(this, userDataStore, pending));
            }

            Provider.onCheckValidWithExplanation += OnPendingPlayerConnecting;
            Provider.onEnemyConnected += OnPlayerConnected;
            Provider.onEnemyDisconnected += OnPlayerDisconnected;
            Provider.onRejectingPlayer += OnRejectingPlayer;
        }

        protected virtual void OnPlayerConnected(SteamPlayer steamPlayer)
        {
            if (m_Players.Any(d => d.SteamPlayer.Equals(steamPlayer)))
            {
                return;
            }

            var pending = m_PendingPlayers.FirstOrDefault(d => d.SteamId == steamPlayer.playerID.steamID);
            if (pending != null)
            {
                FinishSession(pending);
            }

            var player = new UnturnedPlayer(this, m_UserDataStore, steamPlayer.player, pending);
            m_Players.Add(player);

            var connectedEvent = new UserConnectedEvent(player);

            AsyncHelper.RunSync(() => m_EventBus.EmitAsync(m_Runtime, this, connectedEvent));
        }

        protected virtual void OnPlayerDisconnected(SteamPlayer steamPlayer)
        {
            var player = GetPlayer(steamPlayer);

            if (player == null)
            {
                return;
            }

            if (player.Session is UnturnedPlayerSession session)
            {
                session.OnSessionEnd();
            }

            AsyncHelper.RunSync(async () =>
            {
                var disconnectedEvent = new UserDisconnectedEvent(player);
                await m_EventBus.EmitAsync(m_Runtime, this, disconnectedEvent);

                m_Players.Remove(player);

                var userData = await m_UserDataStore.GetUserDataAsync(player.Id, player.Type);
                if (userData == null)
                {
                    return;
                }

                userData.LastSeen = DateTime.Now;
                await m_UserDataStore.SaveUserDataAsync(userData);
            });
        }

        public virtual UnturnedPlayer GetPlayer(Player player)
        {
            return GetPlayer(player.channel.owner);
        }

        public virtual UnturnedPlayer GetPlayer(SteamPlayer player)
        {
            return GetPlayer(player.playerID.steamID);
        }

        public virtual UnturnedPlayer GetPlayer(CSteamID id)
        {
            return m_Players.FirstOrDefault(d => d.SteamId == id);
        }

        protected virtual void OnRejectingPlayer(CSteamID steamId, ESteamRejection rejection, string explanation)
        {
            var pending = m_PendingPlayers.FirstOrDefault(d => d.SteamId == steamId);
            if (pending == null)
            {
                return;
            }

            AsyncHelper.RunSync(async () =>
            {
                var disconnectedEvent = new UserDisconnectedEvent(pending);
                await m_EventBus.EmitAsync(m_Runtime, this, disconnectedEvent);

                FinishSession(pending);

                var userData = await m_UserDataStore.GetUserDataAsync(pending.Id, pending.Type);
                if (userData == null)
                {
                    return;
                }

                userData.LastSeen = DateTime.Now;
                await m_UserDataStore.SaveUserDataAsync(userData);
            });
        }

        protected virtual void OnPendingPlayerConnecting(ValidateAuthTicketResponse_t callback, ref bool isValid, ref string explanation)
        {
            if (m_PendingPlayers.Any(d => d.SteamId == callback.m_SteamID))
            {
                return;
            }

            if (!isValid)
            {
                return;
            }

            //todo check if it is working, if not this should be patched
            var isPendingValid = isValid;
            var rejectExplanation = explanation;

            AsyncHelper.RunSync(async () =>
            {
                var steamPending = Provider.pending.FirstOrDefault(d => d.playerID.steamID == callback.m_SteamID);
                if (steamPending == null)
                {
                    return;
                }

                var pendingPlayer = new UnturnedPendingPlayer(this, m_UserDataStore, steamPending);
                await m_DataSeeder.SeedUserDataAsync(pendingPlayer.Id, pendingPlayer.Type, pendingPlayer.DisplayName);

                var userData = await m_UserDataStore.GetUserDataAsync(pendingPlayer.Id, pendingPlayer.Type);
                if (userData != null)
                {
                    userData.LastSeen = DateTime.Now;
                    userData.LastDisplayName = pendingPlayer.DisplayName;
                    await m_UserDataStore.SaveUserDataAsync(userData);
                }

                m_PendingPlayers.Add(pendingPlayer);

                var userConnectingEvent = new UserConnectingEvent(pendingPlayer);
                await m_EventBus.EmitAsync(m_Runtime, this, userConnectingEvent);

                if (!string.IsNullOrEmpty(userConnectingEvent.RejectionReason))
                {
                    isPendingValid = false;
                    rejectExplanation = userConnectingEvent.RejectionReason;
                }

                if (userConnectingEvent.IsCancelled)
                {
                    isPendingValid = false;
                }
            });

            isValid = isPendingValid;
            explanation = rejectExplanation;
        }

        protected virtual void FinishSession(UnturnedPendingPlayer pending)
        {
            if (pending.Session is UnturnedPendingPlayerSession session)
            {
                session.OnSessionEnd();
            }

            m_PendingPlayers.Remove(pending);
        }

        public bool SupportsUserType(string userType)
        {
            return userType.Equals(KnownActorTypes.Player, StringComparison.OrdinalIgnoreCase);
        }

        public Task<IUser> FindUserAsync(string userType, string searchString, UserSearchMode searchMode)
        {
            var confidence = 0;
            var unturnedUser = (IUser)null;

            foreach (var user in m_Players)
            {
                switch (searchMode)
                {
                    case UserSearchMode.FindByNameOrId:
                    case UserSearchMode.FindById:
                        if (user.Id.Equals(searchString, StringComparison.OrdinalIgnoreCase))
                            return Task.FromResult((IUser)user);

                        if (searchMode == UserSearchMode.FindByNameOrId)
                            goto case UserSearchMode.FindByName;
                        break;

                    case UserSearchMode.FindByName:
                        var currentConfidence = NameConfidence(user.DisplayName, searchString, confidence);
                        if (currentConfidence > confidence)
                        {
                            unturnedUser = user;
                            confidence = currentConfidence;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(searchMode), searchMode, null);
                }
            }

            return Task.FromResult(unturnedUser);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private int NameConfidence(string userName, string searchName, int currentConfidence = -1)
        {
            switch (currentConfidence)
            {
                case 2:
                    if (userName.Equals(searchName, StringComparison.OrdinalIgnoreCase))
                        return 3;
                    goto case 1;

                case 1:
                    if (userName.StartsWith(searchName, StringComparison.OrdinalIgnoreCase))
                        return 2;
                    goto case 0;

                case 0:
                    if (userName.IndexOf(searchName, StringComparison.OrdinalIgnoreCase) != -1)
                        return 1;
                    break;

                default:
                    goto case 2;
            }

            return -1;
        }

        public Task<IReadOnlyCollection<IUser>> GetUsersAsync(string userType)
        {
            return Task.FromResult<IReadOnlyCollection<IUser>>(m_Players);
        }

        public Task BroadcastAsync(string userType, string message, System.Drawing.Color? color)
        {
            if (!KnownActorTypes.Player.Equals(userType, StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            return BroadcastAsync(message, color);
        }

        public Task BroadcastAsync(string message, System.Drawing.Color? color)
        {
            return BroadcastAsync(message, color, isRich: true, iconUrl: Provider.configData.Browser.Icon);
        }

        public Task BroadcastAsync(string message, System.Drawing.Color? color, bool isRich, string iconUrl)
        {
            async UniTask BroadcastTask()
            {
                await UniTask.SwitchToMainThread();

                color ??= System.Drawing.Color.White;

                ChatManager.serverSendMessage(text: message, color: color.Value.ToUnityColor(), useRichTextFormatting: isRich, iconURL: iconUrl);
            }

            return BroadcastTask().AsTask();
        }

        public void Dispose()
        {
            m_Players.Clear();
            // ReSharper disable DelegateSubtraction
            Provider.onCheckValidWithExplanation -= OnPendingPlayerConnecting;
            Provider.onEnemyConnected -= OnPlayerConnected;
            Provider.onEnemyDisconnected -= OnPlayerDisconnected;
            Provider.onRejectingPlayer -= OnRejectingPlayer;
            // ReSharper restore DelegateSubtraction
        }
    }
}