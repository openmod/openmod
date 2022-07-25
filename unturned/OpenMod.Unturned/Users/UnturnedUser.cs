using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Users
{
    /// <summary>
    /// Represents an Unturned user.
    /// </summary>
    public class UnturnedUser : UserBase, IEquatable<UnturnedUser>, IEquatable<UnturnedPendingUser>, IPlayerUser<UnturnedPlayer>
    {
        /// <summary>
        /// See <see cref="SteamId"/>.
        /// </summary>
        public override string Id
        {
            get { return SteamId.ToString(); }
        }

        /// <summary>
        /// The Steam ID of the Unturned user.
        /// </summary>
        public CSteamID SteamId { get; }

        [OpenModInternal]
        protected internal UnturnedUser(
            UnturnedUserProvider userProvider,
            IUserDataStore userDataStore,
            Player player,
            UnturnedPendingUser? pending = null) : base(userProvider, userDataStore)
        {
            var steamPlayerIdId = player.channel.owner.playerID;
            SteamId = steamPlayerIdId.steamID;
            Player = new UnturnedPlayer(player);
            DisplayName = player.channel.owner.playerID.characterName;
            Type = KnownActorTypes.Player;
            Session = new UnturnedUserSession(this, pending?.Session);
        }

        public override Task PrintMessageAsync(string message)
        {
            return PrintMessageAsync(message, System.Drawing.Color.White);
        }

        public override Task PrintMessageAsync(string message, Color color)
        {
            return PrintMessageAsync(message, color, isRich: true, iconUrl: SDG.Unturned.Provider.configData.Browser.Icon);
        }

        public Task PrintMessageAsync(string message, Color color, bool isRich, string iconUrl)
        {
            return Player.PrintMessageAsync(message, color, isRich, iconUrl);
        }

        public bool Equals(UnturnedUser other)
        {
            return !ReferenceEquals(null, other) && (ReferenceEquals(this, other) || other.SteamId.Equals(SteamId));
        }

        public bool Equals(UnturnedPendingUser other)
        {
            return other?.Equals(this) ?? false;
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                UnturnedUser other => Equals(other),
                UnturnedPendingUser otherPending => Equals(otherPending),
                _ => false
            };
        }

        public override int GetHashCode()
        {
            return unchecked((int)(SteamId.m_SteamID * 154 ^ 7 + 165041));
        }

        public UnturnedPlayer Player { get; }

        IPlayer IPlayerUser.Player
        {
            get => Player;
        }
    }
}