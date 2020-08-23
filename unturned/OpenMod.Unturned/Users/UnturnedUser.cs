using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Users
{
    public class UnturnedUser : UserBase, IEquatable<UnturnedUser>, IEquatable<UnturnedPendingUser>, IPlayerUser<UnturnedPlayer>
    {
        public override string Id
        {
            get { return SteamId.ToString(); }
        }

        public CSteamID SteamId { get; }

        [OpenModInternal]
        protected internal UnturnedUser(
            UnturnedUserProvider userProvider,
            IUserDataStore userDataStore,
            Player player,
            UnturnedPendingUser pending = null) : base(userProvider, userDataStore)
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
            async UniTask PrintMessageTask()
            {
                var lines = message.Replace(Environment.NewLine, "\n").Split('\n');
                if (lines.Length == 0)
                {
                    return;
                }

                await UniTask.SwitchToMainThread();

                foreach (var line in lines)
                {
                    var lineToDisplay = line.Trim();
                    if (lineToDisplay.Length == 0)
                    {
                        continue;
                    }

                    foreach (var lline in WrapLine(line))
                    {
                        ChatManager.serverSendMessage(
                            text: lline,
                            color: color.ToUnityColor(),
                            toPlayer: Player.SteamPlayer,
                            iconURL: iconUrl,
                            useRichTextFormatting: isRich);
                    }
                }
            }

            return PrintMessageTask().AsTask();
        }

        private IEnumerable<string> WrapLine(string line)
        {
            var words = line.Split(' ');
            var lines = new List<string>();
            var currentLine = "";
            var maxLength = 90;

            foreach (var currentWord in words)
            {
                if ((currentLine.Length > maxLength) ||
                    ((currentLine.Length + currentWord.Length) > maxLength))
                {
                    lines.Add(currentLine);
                    currentLine = "";
                }

                if (currentLine.Length > 0)
                {
                    currentLine += " " + currentWord;
                }
                else
                {
                    currentLine += currentWord;
                }
            }

            if (currentLine.Length > 0)
            {
                lines.Add(currentLine);
            }

            return lines;
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

        public UnturnedPlayer Player { get; }

        IPlayer IPlayerUser.Player
        {
            get => Player;
        }
    }
}