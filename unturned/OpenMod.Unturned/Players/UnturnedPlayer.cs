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
using System.Globalization;
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
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return other.SteamId.Equals(SteamId);
        }

        public override bool Equals(object obj)
        {
            if (obj is UnturnedPlayer other) return Equals(other);

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

        public string DamageSourceName
        {
            get
            {
                return Player.channel.owner.playerID.characterName;
            }
        }
    }
}