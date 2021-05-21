using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.UnityEngine.Extensions;
using OpenMod.UnityEngine.Helpers;
using OpenMod.UnityEngine.Transforms;
using OpenMod.Unturned.Items;
using OpenMod.Unturned.Vehicles;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector3 = System.Numerics.Vector3;

namespace OpenMod.Unturned.Players
{
    public class UnturnedPlayer : IEquatable<UnturnedPlayer>, IPlayer, IHasHealth, IHasInventory, ICanEnterVehicle, IDamageSource
    {
        public Player Player { get; }

        public SteamPlayer SteamPlayer { get; }

        public CSteamID SteamId { get; }

        [OpenModInternal]
        protected internal UnturnedPlayer(Player player)
        {
            Asset = UnturnedPlayerAsset.Instance;
            State = NullEntityState.Instance;
            Inventory = new UnturnedPlayerInventory(player);
            Transform = new UnityTransform(player.transform);
            Player = player;
            SteamPlayer = Player.channel.owner;
            SteamId = SteamPlayer.playerID.steamID;
            EntityInstanceId = SteamId.ToString();
        }

        public bool Equals(UnturnedPlayer other)
        {
            return ReferenceEquals(this, other) || other.SteamId.Equals(SteamId);
        }

        public override bool Equals(object obj)
        {
            if (obj is UnturnedPlayer other)
                return Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            return unchecked((int)(SteamId.m_SteamID * 174 ^ 5 + 185737));
        }

        public IEntityAsset Asset { get; }

        public IEntityState State { get; }

        public IWorldTransform Transform { get; }

        public string EntityInstanceId { get; }

        public bool IsAlive => !Player.life.isDead;

        public double MaxHealth => 255;

        public double Health => Player.life.health;

        public IPAddress? Address
        {
            get
            {
                return SteamPlayer.getAddress();
            }
        }

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
            async UniTask DamageTask()
            {
                await UniTask.SwitchToMainThread();
                DamageTool.damagePlayer(new DamagePlayerParameters(Player)
                {
                    player = Player,
                    times = 1,
                    damage = (float)amount
                }, out _);
            }

            return DamageTask().AsTask();
        }

        public Task KillAsync()
        {
            return DamageAsync(int.MaxValue / 2f);
        }

        public Task<bool> SetPositionAsync(Vector3 position)
        {
            return SetPositionAsync(position, Player.transform.rotation.ToSystemQuaternion());
        }

        public Task<bool> SetPositionAsync(Vector3 position, Quaternion rotation)
        {
            async UniTask<bool> TeleportationTask()
            {
                await UniTask.SwitchToMainThread();

                if (Player.transform.position == position.ToUnityVector() &&
                    Player.transform.rotation == rotation.ToUnityQuaternion())
                {
                    return true;
                }

                if (!ValidationHelper.IsValid(position) || !ValidationHelper.IsValid(rotation))
                {
                    return false;
                }

                return Player.teleportToLocation(position.ToUnityVector(), rotation.ToEulerAngles().Y);
            }

            return TeleportationTask().AsTask();
        }

        public string Stance => Player.stance.stance.ToString().ToLower(CultureInfo.InvariantCulture);

        IInventory IHasInventory.Inventory => Inventory;

        /// <summary>
        /// Gets the inventory of the player.
        /// </summary>
        public UnturnedPlayerInventory Inventory { get; }

        IVehicle? ICanEnterVehicle.CurrentVehicle => CurrentVehicle;

        /// <summary>
        /// Gets the current vehicle. Returns null if the player is not a passenger.
        /// </summary>
        public UnturnedVehicle? CurrentVehicle
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

        public string DamageSourceName
        {
            get
            {
                return Player.channel.owner.playerID.characterName;
            }
        }

        public Task PrintMessageAsync(string message)
        {
            return PrintMessageAsync(message, Color.White);
        }

        public Task PrintMessageAsync(string message, Color color)
        {
            return PrintMessageAsync(message, color, isRich: true, iconUrl: Provider.configData.Browser.Icon);
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
            var currentLine = new StringBuilder();
            var maxLength = 90;

            foreach (var currentWord in words)
            {
                if (currentLine.Length > maxLength ||
                    currentLine.Length + currentWord.Length > maxLength)
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
    }
}
