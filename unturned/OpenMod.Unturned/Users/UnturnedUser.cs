using System;
using System.Threading.Tasks;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace OpenMod.Unturned.Users
{
    public class UnturnedUser : UserBase, IEquatable<UnturnedUser>, IEquatable<UnturnedPendingUser>
    {
        public override string Id
        {
            get { return SteamId.ToString(); }
        }

        public Player Player { get; }

        public SteamPlayer SteamPlayer { get; }

        public CSteamID SteamId { get; }

        public UnturnedUser(IUserDataStore userDataStore, Player player, UnturnedPendingUser pending) : base(userDataStore)
        {
            Type = KnownActorTypes.Player;
            Player = player;
            SteamPlayer = Player.channel.owner;

            var steamPlayerIdId = SteamPlayer.playerID;
            SteamId = steamPlayerIdId.steamID;
            DisplayName = SteamPlayer.playerID.characterName;

            Session = new UnturnedUserSession(this, pending.Session);
        }

        public override Task PrintMessageAsync(string message)
        {
            ChatManager.serverSendMessage(message, Color.white, toPlayer: SteamPlayer, mode: EChatMode.GLOBAL, useRichTextFormatting: true);
            return Task.CompletedTask;
        }

        public override Task PrintMessageAsync(string message, System.Drawing.Color color)
        {
            var convertedColor = new Color(color.R / 255f, color.G / 255f, color.B / 255f);

            ChatManager.serverSendMessage(message, convertedColor, toPlayer: SteamPlayer, mode: EChatMode.GLOBAL, useRichTextFormatting: true);
            return Task.CompletedTask;
        }

        public bool Equals(UnturnedUser other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return other.SteamId.Equals(SteamId);
        }

        public bool Equals(UnturnedPendingUser other)
        {
            return other?.Equals(this) ?? false;
        }

        public override bool Equals(object obj)
        {
            if (obj is UnturnedUser other)
            {
                return Equals(other);
            }

            if (obj is UnturnedPendingUser otherPending)
            {
                return Equals(otherPending);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return unchecked((int)(SteamId.m_SteamID * 154 ^ 7 + 165041));
        }
    }
}