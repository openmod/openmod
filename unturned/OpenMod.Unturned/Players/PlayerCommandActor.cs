using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Users;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace OpenMod.Unturned.Players
{
    public class PlayerCommandActor : ICommandActor
    {
        public Player Player { get; }

        public SteamPlayer SteamPlayer { get; }

        public SteamPlayerID PlayerId { get; }

        public CSteamID SteamId { get; }

        public string Id
        {
            get { return SteamId.ToString(); }
        }

        public string Type
        {
            get { return KnownUserTypes.Player; }
        }

        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();
        public string DisplayName
        {
            get { return PlayerId.characterName; }
        }

        public PlayerCommandActor(Player player)
        {
            Player = player;
            SteamPlayer = Player.channel.owner;
            PlayerId = SteamPlayer.playerID;
            SteamId = PlayerId.steamID;
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
    }
}