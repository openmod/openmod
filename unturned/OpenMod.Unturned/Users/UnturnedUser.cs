using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.UnityEngine.Extensions;
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
            return PrintMessageAsync(message, iconUrl: null);
        }

        public override Task PrintMessageAsync(string message, Color color)
        {
            return PrintMessageAsync(message, color, iconUrl: null);
        }

        public override Task PrintMessageAsync(string message, string? iconUrl)
        {
            return PrintMessageAsync(message, Color.White, iconUrl);
        }

        public override Task PrintMessageAsync(string message, Color color, string? iconUrl)
        {
            iconUrl ??= SDG.Unturned.Provider.configData.Browser.Icon;
            return PrintMessageAsync(message, color, isRich: true, iconUrl);
        }

        public Task PrintMessageAsync(string message, Color color, bool isRich, string iconUrl)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            async UniTask PrintMessageTask()
            {
                var lines = message.Replace(System.Environment.NewLine, "\n").Split('\n');
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
            var currentLine = new StringBuilder();
            const int c_MaxLength = 90;

            foreach (var currentWord in words)
            {
                if ((currentLine.Length > c_MaxLength) ||
                    ((currentLine.Length + currentWord.Length) > c_MaxLength))
                {
                    lines.Add(currentLine.ToString());
                    currentLine.Clear();
                }

                if (currentLine.Length > 0)
                {
                    currentLine.Append(" ");
                    currentLine.Append(currentWord);
                }
                else
                {
                    currentLine.Append(currentWord);
                }
            }

            if (currentLine.Length > 0)
            {
                lines.Add(currentLine.ToString());
            }

            return lines;
        }

        public bool Equals(UnturnedUser other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

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