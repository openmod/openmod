using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Localization;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Users;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Users.Events;
using SDG.Unturned;
using Steamworks;
using Nito.AsyncEx;

namespace OpenMod.Unturned.Users
{
    [Priority(Priority = Priority.Low)]
    public class UnturnedUserProvider : IUserProvider, IDisposable
    {
        private readonly HashSet<UnturnedUser> m_Users;
        private readonly HashSet<UnturnedPendingUser> m_PendingUsers;

        private readonly IEventBus m_EventBus;
        private readonly IOpenModStringLocalizer m_StringLocalizer;
        private readonly IRuntime m_Runtime;
        private readonly IUserDataSeeder m_DataSeeder;

        private readonly IUserDataStore m_UserDataStore;

        public UnturnedUserProvider(
            IEventBus eventBus,
            IOpenModStringLocalizer stringLocalizer,
            IUserDataSeeder dataSeeder,
            IUserDataStore userDataStore,
            IRuntime runtime)
        {
            m_EventBus = eventBus;
            m_DataSeeder = dataSeeder;
            m_StringLocalizer = stringLocalizer;
            m_UserDataStore = userDataStore;
            m_Runtime = runtime;
            m_Users = new HashSet<UnturnedUser>();
            m_PendingUsers = new HashSet<UnturnedPendingUser>();

            // sync users in case of openmod reloads
            if (Provider.clients != null)
            {
                foreach (var client in Provider.clients)
                {
                    m_Users.Add(new UnturnedUser(this, userDataStore, client.player));
                }
            }

            if (Provider.pending != null)
            {
                foreach (var pending in Provider.pending)
                {
                    m_PendingUsers.Add(new UnturnedPendingUser(this, userDataStore, pending));
                }
            }

            Provider.onCheckValidWithExplanation += OnPendingPlayerConnecting;
            Provider.onServerConnected += OnPlayerConnected;
            Provider.onServerDisconnected += OnPlayerDisconnected;
            Provider.onRejectingPlayer += OnRejectingPlayer;
        }

        protected virtual void OnPlayerConnected(CSteamID steamID)
        {
            var pending = m_PendingUsers.FirstOrDefault(d => d.SteamId == steamID);
            if (pending != null)
            {
                FinishSession(pending);
            }

            var steamPlayer = PlayerTool.getSteamPlayer(steamID);
            if (steamPlayer == null)
            {
                return;
            }

            var user = new UnturnedUser(this, m_UserDataStore, steamPlayer.player, pending);
            m_Users.Add(user);

            AsyncContext.Run(async () =>
            {
                var @event = new UnturnedUserConnectedEvent(user);
                await m_EventBus.EmitAsync(m_Runtime, this, @event);
            });
        }

        protected virtual void OnPlayerDisconnected(CSteamID steamID)
        {
            var user = GetUser(steamID);

            if (user == null)
            {
                return;
            }

            if (user.Session is UnturnedUserSession session)
            {
                session.OnSessionEnd();
            }

            AsyncHelper.RunSync(async () =>
            {
                var disconnectedEvent = new UnturnedUserDisconnectedEvent(user);
                await m_EventBus.EmitAsync(m_Runtime, this, disconnectedEvent);
            });

            m_Users.Remove(user);
            UpdateLastSeen(user.Id, user.Type);
        }

        public virtual UnturnedUser GetUser(Player player)
        {
            return GetUser(player.channel.owner);
        }

        public virtual UnturnedUser GetUser(SteamPlayer player)
        {
            return GetUser(player.playerID.steamID)!;
        }

        public virtual UnturnedUser? GetUser(CSteamID id)
        {
            return m_Users.FirstOrDefault(d => d.SteamId == id);
        }

        protected virtual void OnRejectingPlayer(CSteamID steamId, ESteamRejection rejection, string explanation)
        {
            var pending = m_PendingUsers.FirstOrDefault(d => d.SteamId == steamId);
            if (pending == null)
            {
                return;
            }

            //This doesn't affect event
            FinishSession(pending);

            AsyncHelper.RunSync(async () =>
            {
                var disconnectedEvent = new UnturnedPendingUserDisconnectedEvent(pending);
                await m_EventBus.EmitAsync(m_Runtime, this, disconnectedEvent);
            });

            UpdateLastSeen(pending.Id, pending.Type);
        }

        private void UpdateLastSeen(string id, string type)
        {
            UniTask.RunOnThreadPool(async () =>
            {
                var userData = await m_UserDataStore.GetUserDataAsync(id, type);
                if (userData == null)
                {
                    return;
                }

                userData.LastSeen = DateTime.Now;
                await m_UserDataStore.SetUserDataAsync(userData);
            }).Forget();
        }

        protected virtual void OnPendingPlayerConnecting(ValidateAuthTicketResponse_t callback, ref bool isValid,
            ref string? explanation)
        {
            var user = GetUser(callback.m_SteamID);
            if (user != null || m_PendingUsers.Any(d => d.SteamId == callback.m_SteamID))
            {
                return;
            }

            var steamPending = Provider.pending.FirstOrDefault(d => d.playerID.steamID == callback.m_SteamID);
            if (steamPending == null)
            {
                return;
            }

            var pendingUser = new UnturnedPendingUser(this, m_UserDataStore, steamPending);
            m_PendingUsers.Add(pendingUser);

            var isPendingValid = isValid;
            var rejectExplanation = explanation;
            AsyncHelper.RunSync(async () =>
            {
                UnturnedUserConnectingEvent userEvent;

                var userData = await m_UserDataStore.GetUserDataAsync(pendingUser.Id, pendingUser.Type);
                if (userData != null)
                {
                    userEvent = new UnturnedUserConnectingEvent(pendingUser);

                    userData.LastSeen = DateTime.Now;
                    userData.LastDisplayName = pendingUser.DisplayName;
                    UniTask.RunOnThreadPool(() => m_UserDataStore.SetUserDataAsync(userData)).Forget();
                }
                else
                {
                    userEvent = new UnturnedUserFirstConnectingEvent(pendingUser);
                    UniTask.RunOnThreadPool(() => m_DataSeeder.SeedUserDataAsync(pendingUser.Id, pendingUser.Type, pendingUser.DisplayName)).Forget();
                }

                userEvent.IsCancelled = !isPendingValid;
                if (rejectExplanation != null)
                {
                    await userEvent.RejectAsync(rejectExplanation);
                }

                await m_EventBus.EmitAsync(m_Runtime, this, userEvent);
                if (!string.IsNullOrEmpty(userEvent.RejectionReason))
                {
                    isPendingValid = false;
                    rejectExplanation = userEvent.RejectionReason;
                }
                else if (userEvent.IsCancelled)
                {
                    isPendingValid = false;
                }
            });
            isValid = isPendingValid;
            explanation = rejectExplanation;
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

        public Task<IUser?> FindUserAsync(string userType, string searchString, UserSearchMode searchMode)
        {
            var confidence = 0;
            var unturnedUser = (IUser?)null;

            foreach (var user in m_Users)
            {
                switch (searchMode)
                {
                    case UserSearchMode.FindByNameOrId:
                    case UserSearchMode.FindById:
                        if (user.Id.Equals(searchString, StringComparison.OrdinalIgnoreCase))
                            return Task.FromResult((IUser?)user);

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
            return Task.FromResult<IReadOnlyCollection<IUser>>(m_Users);
        }

        public Task BroadcastAsync(string userType, string message, System.Drawing.Color? color)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
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

        public Task<bool> BanAsync(IUser user, string? reason = null, DateTime? expireDate = null)
        {
            return BanAsync(user, instigator: null, reason, expireDate);
        }

        public async Task<bool> BanAsync(IUser user, IUser? instigator = null, string? reason = null, DateTime? expireDate = null)
        {
            expireDate ??= DateTime.MaxValue;

            var duration = (expireDate.Value - DateTime.Now).TotalSeconds;
            if (duration <= 0)
                return false;

            reason ??= m_StringLocalizer["ban_default"];
            var data = await m_UserDataStore.GetUserDataAsync(user.Id, user.Type);
            if (data != null)
            {
                data.BanInfo = new BanData(reason!, instigator, expireDate);
                await m_UserDataStore.SetUserDataAsync(data);
            }

            await UniTask.SwitchToMainThread();
            Provider.ban(new CSteamID(ulong.Parse(user.Id)), reason, duration > uint.MaxValue ? SteamBlacklist.PERMANENT : (uint)duration);
            return true;
        }

        public Task<bool> KickAsync(IUser user, string? reason = null)
        {
            if (user is not UnturnedUser player)
            {
                return Task.FromResult(result: false);
            }

            reason ??= "No reason provided";
            async UniTask<bool> KickTask()
            {
                await UniTask.SwitchToMainThread();
                Provider.kick(player.SteamId, reason);
                return true;
            }

            return KickTask().AsTask();
        }

        public ICollection<UnturnedUser> GetOnlineUsers()
        {
            return m_Users;
        }

        public ICollection<UnturnedPendingUser> GetPendingUsers()
        {
            return m_PendingUsers;
        }

        public void Dispose()
        {
            m_Users.Clear();
            m_PendingUsers.Clear();

            Provider.onCheckValidWithExplanation -= OnPendingPlayerConnecting;
            Provider.onServerConnected -= OnPlayerConnected;
            Provider.onServerDisconnected -= OnPlayerDisconnected;
            Provider.onRejectingPlayer -= OnRejectingPlayer;
        }
    }
}
