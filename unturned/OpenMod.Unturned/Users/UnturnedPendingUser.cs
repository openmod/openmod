using OpenMod.API;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using SDG.Unturned;
using Steamworks;
using System;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Users
{
    /// <summary>
    /// Represents a pending unturned user.
    /// </summary>
    public class UnturnedPendingUser : UserBase, IEquatable<UnturnedPendingUser>, IEquatable<UnturnedUser>
    {
        /// <summary>
        /// See <see cref="SteamId"/>.
        /// </summary>
        public override string Id
        {
            get => SteamId.ToString();
        }

        /// <summary>
        /// The Steam ID of the user.
        /// </summary>
        public CSteamID SteamId { get; }

        /// <summary>
        /// The wrapped SteamPending.
        /// </summary>
        public SteamPending SteamPending { get; }

        [OpenModInternal]
        public UnturnedPendingUser(IUserProvider userProvider, IUserDataStore userDataStore, SteamPending steamPending) : base(userProvider, userDataStore)
        {
            SteamPending = steamPending;
            SteamId = steamPending.playerID.steamID;
            DisplayName = steamPending.playerID.characterName;
            Type = KnownActorTypes.Player;
            Session = new UnturnedPendingUserSession(this);
        }

        public override Task PrintMessageAsync(string message)
        {
            return Task.CompletedTask;
        }

        public override Task PrintMessageAsync(string message, System.Drawing.Color color)
        {
            return Task.CompletedTask;
        }

        public bool Equals(UnturnedPendingUser? other)
        {
            if (other is null)
            {
                return false;
            }

            return ReferenceEquals(this, other) || SteamId.Equals(other.SteamId);
        }

        public bool Equals(UnturnedUser? other)
        {
            return other?.SteamId == SteamId;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj switch
            {
                UnturnedUser user => Equals(user),
                UnturnedPendingUser pendingUser => Equals(pendingUser),
                _ => false
            };
        }

        public override int GetHashCode()
        {
            return SteamId.GetHashCode();
        }
    }
}