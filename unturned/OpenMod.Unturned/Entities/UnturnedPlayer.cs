using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.UnityEngine.Extensions;
using OpenMod.UnityEngine.Helpers;
using OpenMod.UnityEngine.Transforms;
using OpenMod.Unturned.Items;
using OpenMod.Unturned.Users;
using OpenMod.Unturned.Vehicles;
using SDG.Unturned;
using Steamworks;
using IHasInventory = OpenMod.Extensions.Games.Abstractions.Entities.IHasInventory;
using Vector3 = System.Numerics.Vector3;

namespace OpenMod.Unturned.Entities
{
    public class UnturnedPlayer : UserBase, IEquatable<UnturnedPlayer>, IEquatable<UnturnedPendingPlayer>, 
        IPlayer, IHasHealth, IHasInventory, ICanEnterVehicle
    {
        public override string Id
        {
            get { return SteamId.ToString(); }
        }

        public Player Player { get; }

        public SteamPlayer SteamPlayer { get; }

        public CSteamID SteamId { get; }

        [OpenModInternal]
        protected internal UnturnedPlayer(
            UnturnedUserProvider userProvider,
            IUserDataStore userDataStore,
            Player player,
            UnturnedPendingPlayer pending = null) : base(userProvider, userDataStore)
        {
            Asset = UnturnedPlayerAsset.Instance;
            State = NullEntityState.Instance;
            Inventory = new UnturnedPlayerInventory(Player);
            Transform = new UnityTransform(player.transform);
            Player = player;
            SteamPlayer = Player.channel.owner;

            var steamPlayerIdId = SteamPlayer.playerID;
            SteamId = steamPlayerIdId.steamID;
            EntityInstanceId = SteamId.ToString();

            DisplayName = SteamPlayer.playerID.characterName;
            Type = KnownActorTypes.Player;
            Session = new UnturnedPlayerSession(this, pending?.Session);
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
                            toPlayer: SteamPlayer,
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

        public bool Equals(UnturnedPlayer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return other.SteamId.Equals(SteamId);
        }

        public bool Equals(UnturnedPendingPlayer other)
        {
            return other?.Equals(this) ?? false;
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                UnturnedPlayer other => Equals(other),
                UnturnedPendingPlayer otherPending => Equals(otherPending),
                _ => false
            };
        }

        public override int GetHashCode()
        {
            return unchecked((int)(SteamId.m_SteamID * 154 ^ 7 + 165041));
        }

        public IEntityAsset Asset { get; }

        public IEntityState State { get; }

        public IWorldTransform Transform { get; }

        public string EntityInstanceId { get; }

        public bool IsAlive => !Player.life.isDead;

        public double MaxHealth => 255;

        public double Health => Player.life.health;

        public Task SetHealthAsync(double health)
        {
            async UniTask SetHealthTask()
            {
                await UniTask.SwitchToMainThread();
                Player.life.askHeal((byte)health, Player.life.isBleeding, Player.life.isBroken);
            }

            return SetHealthTask().AsTask();
        }

        public Task DamageAsync(double amount)
        {
            async UniTask SetHealthTask()
            {
                await UniTask.SwitchToMainThread();
                DamageTool.damagePlayer(new DamagePlayerParameters
                {
                    player = Player,
                    times = 1,
                    damage = (float)amount
                }, out _);
            }

            return SetHealthTask().AsTask();
        }

        public Task KillAsync()
        {
            return DamageAsync(float.MaxValue);
        }

        public Task<bool> SetPositionAsync(Vector3 position)
        {
            return SetPositionAsync(position, Player.transform.rotation.eulerAngles.ToSystemVector());
        }

        public Task<bool> SetPositionAsync(Vector3 position, Vector3 rotation)
        {
            async UniTask<bool> TeleportationTask()
            {
                await UniTask.SwitchToMainThread();
                
                if (Player.transform.position == position.ToUnityVector() &&
                    Player.transform.rotation.eulerAngles == rotation.ToUnityVector())
                {
                    return true;
                }

                if (!ValidationHelper.IsValid(position) || !ValidationHelper.IsValid(rotation))
                {
                    return false;
                }

                var rotationAngle = MeasurementTool.angleToByte(rotation.Y);
                Player.channel.send("askTeleport", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, rotationAngle);
                return true;
            }

            return TeleportationTask().AsTask();
        }

        public string Stance => Player.stance.stance.ToString().ToLower(CultureInfo.InvariantCulture);

        public IInventory Inventory { get; }

        public IVehicle CurrentVehicle
        {
            get
            {
                var vehicle = Player.movement.getVehicle();
                if (vehicle == null)
                {
                    return null;
                }

                return new UnturnedVehicle(vehicle);
            }
        }
    }
}