using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.Core.Users;
using OpenMod.Unturned.API;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace OpenMod.Unturned.Commands
{
    public class UnturnedPlayerCommandActor : IUnturnedPlayerActor
    {
        public Player Player { get; }
        public SteamPlayer SteamPlayer { get; }
        public CSteamID SteamId { get; }
        public string DisplayName { get; }


        public string Id => SteamId.ToString();
        public string Type => KnownUserTypes.Player;


        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();
    

        public UnturnedPlayerCommandActor(Player player)
        {
            Player = player;
            SteamPlayer = Player.channel.owner;

            var steamPlayerIdId = SteamPlayer.playerID;
            SteamId = steamPlayerIdId.steamID;
            DisplayName = SteamPlayer.playerID.characterName;
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