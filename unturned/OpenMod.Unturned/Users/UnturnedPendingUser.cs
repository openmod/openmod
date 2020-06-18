using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Users
{
    public class UnturnedPendingUser : IUser, IEquatable<UnturnedPendingUser>, IEquatable<UnturnedUser>
    {
        public string Id
        {
            get { return SteamId.ToString(); }
        }

        public string Type { get; } = KnownActorTypes.Player;

        public string DisplayName { get; }

        public CSteamID SteamId { get; }

        public SteamPending SteamPending { get; }

        public bool IsOnline { get; } = false;

        public DateTime? SessionStartTime { get; }

        public DateTime? SessionEndTime { get; internal set; }

        public UnturnedPendingUser(SteamPending steamPending)
        {
            SessionStartTime = DateTime.Now;
            SteamPending = steamPending;
            SteamId = steamPending.playerID.steamID;
            DisplayName = steamPending.playerID.characterName;
        }

        public Task PrintMessageAsync(string message)
        {
            throw new NotSupportedException();
        }

        public Task PrintMessageAsync(string message, System.Drawing.Color color)
        {
            throw new NotSupportedException();
        }


        public Task DisconnectAsync(string reason = "")
        {
            Provider.reject(SteamId, ESteamRejection.PLUGIN, reason ?? string.Empty);
            return Task.CompletedTask;
        }

        public Dictionary<string, object> SessionData { get; } = new Dictionary<string, object>();

        public Dictionary<string, object> PersistentData { get; internal set; }

        public bool Equals(UnturnedPendingUser other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return SteamId.Equals(other.SteamId);
        }

        public bool Equals(UnturnedUser other)
        {
            return other?.SteamId == SteamId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (obj is UnturnedUser user)
            {
                return Equals(user);
            }

            if (obj is UnturnedPendingUser pendingUser)
            {
                return Equals(pendingUser);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return SteamId.GetHashCode();
        }
    }
}