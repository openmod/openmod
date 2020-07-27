using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API;
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

        [OpenModInternal]
        public UnturnedUser(IUserDataStore userDataStore, Player player, UnturnedPendingUser pending = null) : base(userDataStore)
        {
            Type = KnownActorTypes.Player;
            Player = player;
            SteamPlayer = Player.channel.owner;

            var steamPlayerIdId = SteamPlayer.playerID;
            SteamId = steamPlayerIdId.steamID;
            DisplayName = SteamPlayer.playerID.characterName;

            Session = new UnturnedUserSession(this, pending?.Session);
        }

        public override Task PrintMessageAsync(string message)
        {
            return PrintMessageAsync(message, System.Drawing.Color.White);
        }

        public override Task PrintMessageAsync(string message, System.Drawing.Color color)
        {
            async UniTask PrintMessageTask()
            {
                var convertedColor = new Color(color.R / 255f, color.G / 255f, color.B / 255f);

                var lines = message.Split('\n');
                await UniTask.SwitchToMainThread();
                foreach (var line in lines)
                {
                    var lineToDisplay = line.Trim();
                    if (lineToDisplay.Length == 0)
                        continue;

                    ChatManager.serverSendMessage(
                        text: line,
                        color: convertedColor,
                        toPlayer: SteamPlayer,
                        iconURL: Provider.configData.Browser.Icon,
                        useRichTextFormatting: true);
                }
            }

            return PrintMessageTask().AsTask();
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
    }
}