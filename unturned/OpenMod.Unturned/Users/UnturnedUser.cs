using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace OpenMod.Unturned.Users
{
    public class UnturnedUser : IUser, IEquatable<UnturnedUser>, IEquatable<UnturnedPendingUser>
    {
        public string Id
        {
            get { return SteamId.ToString(); }
        }

        public string Type { get; } = KnownActorTypes.Player;

        public Player Player { get; }

        public SteamPlayer SteamPlayer { get; }

        public CSteamID SteamId { get; }

        public string DisplayName { get; }

        public bool IsOnline { get; internal set; }

        public DateTime? SessionStartTime { get; internal set; }

        public DateTime? SessionEndTime { get; internal set; }

        public UnturnedUser(Player player, UnturnedPendingUser pending)
        {
            Player = player;
            SteamPlayer = Player.channel.owner;

            var steamPlayerIdId = SteamPlayer.playerID;
            SteamId = steamPlayerIdId.steamID;
            DisplayName = SteamPlayer.playerID.characterName;

            SessionData = pending.SessionData;
            PersistentData = pending.PersistentData;
            SessionStartTime = pending.SessionStartTime;
        }

        public Task PrintMessageAsync(string message)
        {
            ChatManager.serverSendMessage(message, Color.white, toPlayer: SteamPlayer, mode: EChatMode.GLOBAL, useRichTextFormatting: true);
            return Task.CompletedTask;
        }

        public Task PrintMessageAsync(string message, System.Drawing.Color color)
        {
            var convertedColor = new Color(color.R / 255f, color.G / 255f, color.B / 255f);

            ChatManager.serverSendMessage(message, convertedColor, toPlayer: SteamPlayer, mode: EChatMode.GLOBAL, useRichTextFormatting: true);
            return Task.CompletedTask;
        }

        public Task DisconnectAsync(string reason = "")
        {
            Provider.kick(SteamId, reason ?? string.Empty);
            return Task.CompletedTask;
        }

        public Dictionary<string, object> SessionData { get; internal set; }

        public Dictionary<string, object> PersistentData { get; }

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