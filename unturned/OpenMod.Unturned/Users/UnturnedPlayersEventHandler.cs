using System;
using System.Collections.Generic;
using System.Linq;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Users.Events;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Users
{
    public class UnturnedPlayerEventsHandler : IDisposable
    {
        private readonly HashSet<UnturnedUser> m_UnturnedUsers;
        private readonly HashSet<UnturnedPendingUser> m_PendingUsers;

        private readonly IEventBus m_EventBus;
        private readonly ICommandExecutor m_CommandExecutor;
        private readonly IOpenModHost m_Host;
        private readonly IUserDataSeeder m_DataSeeder;
        private readonly IUserDataStore m_UserDataStore;
        private bool m_Subscribed;

        public UnturnedPlayerEventsHandler(
            IEventBus eventBus,
            ICommandExecutor commandExecutor,
            IOpenModHost host,
            IUserDataSeeder dataSeeder,
            IUserDataStore userDataStore)
        {
            m_EventBus = eventBus;
            m_CommandExecutor = commandExecutor;
            m_Host = host;
            m_DataSeeder = dataSeeder;
            m_UserDataStore = userDataStore;
            m_UnturnedUsers = new HashSet<UnturnedUser>();
            m_PendingUsers = new HashSet<UnturnedPendingUser>();
        }

        public virtual void Subscribe()
        {
            if (m_Subscribed)
            {
                return;
            }

            Provider.onCheckValidWithExplanation += OnPendingPlayerConnected;
            Provider.onEnemyConnected += OnPlayerConnected;
            Provider.onEnemyDisconnected += OnEnemyDisconnected;
            ChatManager.onCheckPermissions += OnCheckCommandPermissions;

            m_Subscribed = true;
        }

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
                var pendingUser = new UnturnedPendingUser(steamPending);
                await m_DataSeeder.SeedUserDataAsync(pendingUser.Id, pendingUser.Type, pendingUser.DisplayName);

                var userData = await m_UserDataStore.GetUserDataAsync(pendingUser.Type, pendingUser.Id);
                userData.LastSeen = DateTime.Now;
                userData.LastDisplayName = pendingUser.DisplayName;
                await m_UserDataStore.SaveUserDataAsync(userData);

                pendingUser.PersistentData = userData.Data;
                m_PendingUsers.Add(pendingUser);

                var userConnectingEvent = new UserConnectingEvent(pendingUser);
                await m_EventBus.EmitAsync(m_Host, this, userConnectingEvent);

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

        // todo: memory leak, m_PendingUsers does not get cleared up when pending user gets rejected
        // Unturned does not have an event for handling rejections yet.


        protected virtual void OnPlayerConnected(SteamPlayer player)
        {
            AsyncHelper.RunSync(async () =>
            {
                var pending = m_PendingUsers.First(d => d.SteamId == player.playerID.steamID);
                FinishSession(pending);

                var user = new UnturnedUser(player.player, pending)
                {
                    SessionStartTime = DateTime.Now,
                };

                m_UnturnedUsers.Add(user);

                var connectedEvent = new UserConnectedEvent(user);
                await m_EventBus.EmitAsync(m_Host, this, connectedEvent);
            });
        }

        protected virtual void FinishSession(UnturnedPendingUser pending)
        {
            pending.SessionEndTime = DateTime.Now;
            m_PendingUsers.Remove(pending);
        }

        protected virtual void OnEnemyDisconnected(SteamPlayer player)
        {
            AsyncHelper.RunSync(async () =>
            {
                var user = GetUser(player);
                user.SessionEndTime = DateTime.Now;
                var disconnectedEvent = new UserDisconnectedEvent(user);

                await m_EventBus.EmitAsync(m_Host, this, disconnectedEvent);
                m_UnturnedUsers.Remove(user);

                var userData = await m_UserDataStore.GetUserDataAsync(user.Id, user.Type);
                userData.Data = user.PersistentData;
                userData.LastSeen = DateTime.Now;
                await m_UserDataStore.SaveUserDataAsync(userData);
            });
        }

        protected virtual void OnCheckCommandPermissions(SteamPlayer player, string text, ref bool shouldExecuteCommand, ref bool shouldList)
        {
            if (!shouldExecuteCommand || !text.StartsWith("/"))
            {
                return;
            }

            shouldExecuteCommand = false;
            shouldList = false;

            var actor = GetUser(player);
            AsyncHelper.Schedule("Player command execution", () => m_CommandExecutor.ExecuteAsync(actor, text.Split(' '), string.Empty));
        }


        public virtual void Unsubscribe()
        {
            if (!m_Subscribed)
            {
                return;
            }

            // ReSharper disable DelegateSubtraction
            Provider.onEnemyConnected -= OnPlayerConnected;
            Provider.onEnemyDisconnected -= OnEnemyDisconnected;
            ChatManager.onCheckPermissions -= OnCheckCommandPermissions;
            Provider.onCheckValidWithExplanation -= OnPendingPlayerConnected;
            // ReSharper restore DelegateSubtraction

            m_Subscribed = false;
        }

        public virtual void Dispose()
        {
            Unsubscribe();
        }

        protected virtual UnturnedUser GetUser(Player player)
        {
            return GetUser(player.channel.owner);
        }

        protected virtual UnturnedUser GetUser(SteamPlayer player)
        {
            return GetUser(player.playerID.steamID);
        }

        protected virtual UnturnedUser GetUser(CSteamID id)
        {
            return m_UnturnedUsers.FirstOrDefault(d => d.SteamId == id);
        }
    }
}