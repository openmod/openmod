using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.Core.Users;
using OpenMod.Unturned.API;
using OpenMod.Unturned.API.Player;
using OpenMod.Unturned.World;
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
        
        public PlayerDeath LastDeath { get; set; }

        /// <inheritdoc />
        public async Task<float> GetDistanceFrom(Vector3 point) => Vector3.Distance(Player.transform.position, point);
        
        public string DisplayName { get; }
        
        
        public string Id
        {
            get { return SteamId.ToString(); }
        }
        public string Type
        {
            get { return KnownActorTypes.Player; }
        }


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