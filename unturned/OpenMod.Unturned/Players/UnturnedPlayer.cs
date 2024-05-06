﻿using Cysharp.Threading.Tasks;
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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vector3 = System.Numerics.Vector3;

namespace OpenMod.Unturned.Players
{
    //todo add infect and oxygen
    public class UnturnedPlayer : IEquatable<UnturnedPlayer>, IPlayer, IHasHealth, IHasInventory, ICanEnterVehicle, IDamageSource, IHasHunger, IHasThirst
    {
        public Player Player { get; }

        public SteamPlayer SteamPlayer { get; }

        public CSteamID SteamId { get; }

        public PlayerLife PlayerLife { get; }

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

            PlayerLife = Player.life;
        }

        public bool Equals(UnturnedPlayer other)
        {
            return ReferenceEquals(this, other) || other.SteamId.Equals(SteamId);
        }

        public override bool Equals(object? obj)
        {
            if (obj is UnturnedPlayer other)
            {
                return Equals(other);
            }

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

        public bool IsAlive
        {
            get => !PlayerLife.isDead;
        }

        public double MaxHealth
        {
            get => 100; //Unturned Max health
        }

        public double Health
        {
            get => PlayerLife.health;
        }

        public IPAddress? Address
        {
            get => SteamPlayer.getAddress();
        }

        public Task SetFullHealthAsync()
        {
            return SetFullHealthTask().AsTask();

            async UniTask SetFullHealthTask()
            {
                await UniTask.SwitchToMainThread();

                PlayerLife.askHeal((byte)MaxHealth, PlayerLife.isBleeding, PlayerLife.isBroken);

                //todo add infect and oxygen
                PlayerLife.askEat((byte)MaxHunger);
                PlayerLife.askDrink((byte)MaxThirst);
            }
        }

        public Task SetHealthAsync(double health)//This is a Set and NOT a incremental
        {
            return SetHealthTask().AsTask();

            async UniTask SetHealthTask()
            {
                if (health < 0 || health > MaxHealth)
                {
                    throw new ArgumentException($"Invalid health amount({health}). It must be lower than {MaxHealth} and above 0.");
                }

                await UniTask.SwitchToMainThread();

                var amount = (float)health - PlayerLife.health;
                if (amount == 0)
                {
                    return;
                }

                PlayerLife.serverModifyHealth(amount);
            }
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
            return SetPositionAsync(position, Transform.Rotation);
        }

        public Task<bool> SetPositionAsync(Vector3 position, Quaternion rotation)
        {
            async UniTask<bool> TeleportationTask()
            {
                await UniTask.SwitchToMainThread();

                if (Transform.Position == position && Transform.Rotation == rotation)
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

        public string Stance
        {
            get => Player.stance.stance.ToString().ToLower(CultureInfo.InvariantCulture);
        }

        IInventory IHasInventory.Inventory
        {
            get => Inventory;
        }

        /// <summary>
        /// Gets the inventory of the player.
        /// </summary>
        public UnturnedPlayerInventory Inventory { get; }

        IVehicle? ICanEnterVehicle.CurrentVehicle
        {
            get => CurrentVehicle;
        }

        /// <summary>
        /// Gets the current vehicle. Returns null if the player is not a passenger.
        /// </summary>
        public UnturnedVehicle? CurrentVehicle
        {
            get
            {
                var vehicle = Player.movement.getVehicle();
                return vehicle == null ? null : new UnturnedVehicle(vehicle);
            }
        }

        public string DamageSourceName
        {
            get => SteamPlayer.playerID.characterName;
        }

        public double MaxHunger
        {
            get => 100; //Unturned Max hunger
        }

        public double Hunger
        {
            get => PlayerLife.food;
        }

        public double MaxThirst
        {
            get => 100; //Unturned Max Thirst
        }

        public double Thirst
        {
            get => PlayerLife.water;
        }

        [UsedImplicitly]
        public Task PrintMessageAsync(string message)
        {
            return PrintMessageAsync(message, Color.White);
        }

        // ReSharper disable once MemberCanBePrivate.Global
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

            return PrintMessageTask().AsTask();

            async UniTask PrintMessageTask()
            {
                var lines = message.Replace(System.Environment.NewLine, "\n").Split('\n').Where(line => line.Trim().Length > 0).ToArray();
                if (!lines.Any())
                {
                    return;
                }

                await UniTask.SwitchToMainThread();

                foreach (var line in lines)
                {
                    ChatManager.serverSendMessage(
                        text: line,
                        color: color.ToUnityColor(),
                        toPlayer: SteamPlayer,
                        iconURL: iconUrl,
                        useRichTextFormatting: isRich);
                }
            }
        }

        public Task SetHungerAsync(double hunger)//This is a Set and NOT a incremental
        {
            async UniTask SetHungerTask()
            {
                if (hunger < 0 || hunger > MaxHunger)
                {
                    throw new ArgumentException($"Invalid hunger amount({hunger}). It must be lower than {MaxHunger} and above 0.");
                }

                await UniTask.SwitchToMainThread();

                var amount = (float)hunger - PlayerLife.food;
                if (amount == 0)
                {
                    return;
                }

                PlayerLife.serverModifyFood(amount);
            }

            return SetHungerTask().AsTask();
        }

        public Task SetThirstAsync(double thirst)//This is a Set and NOT a incremental
        {
            async UniTask SetThirstTask()
            {
                if (thirst < 0 || thirst > MaxThirst)
                {
                    throw new ArgumentException($"Invalid thirst amount({thirst}). It must be lower than {MaxThirst} and above 0.");
                }

                await UniTask.SwitchToMainThread();

                var amount = (float)thirst - PlayerLife.water;
                if (amount == 0)
                {
                    return;
                }

                PlayerLife.serverModifyWater(amount);
            }

            return SetThirstTask().AsTask();
        }
    }
}
