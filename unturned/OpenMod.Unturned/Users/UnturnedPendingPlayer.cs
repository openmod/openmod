using System;
using System.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Users
{
    public class UnturnedPendingPlayer : UserBase, IEquatable<UnturnedPendingPlayer>, IEquatable<UnturnedPlayer>
    {
        public override string Id
        {
            get { return SteamId.ToString(); }
        }

        public CSteamID SteamId { get; }

        public SteamPending SteamPending { get; }

        [OpenModInternal]
        public UnturnedPendingPlayer(IUserProvider userProvider, IUserDataStore userDataStore, SteamPending steamPending) : base(userProvider, userDataStore)
        {
            SteamPending = steamPending;
            SteamId = steamPending.playerID.steamID;
            DisplayName = steamPending.playerID.characterName;
            Type = KnownActorTypes.Player;
            Session = new UnturnedPendingPlayerSession(this);
        }

        public override Task PrintMessageAsync(string message)
        {
            return Task.CompletedTask;
        }

        public override Task PrintMessageAsync(string message, System.Drawing.Color color)
        {
            return Task.CompletedTask;
        }

        public bool Equals(UnturnedPendingPlayer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return SteamId.Equals(other.SteamId);
        }

        public bool Equals(UnturnedPlayer other)
        {
            return other?.SteamId == SteamId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (obj is UnturnedPlayer user)
            {
                return Equals(user);
            }

            if (obj is UnturnedPendingPlayer pendingUser)
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