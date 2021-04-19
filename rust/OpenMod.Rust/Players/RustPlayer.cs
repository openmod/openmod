using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Rust.Entities;
using OpenMod.Rust.Items;
using OpenMod.UnityEngine.Extensions;
using System;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using IHasInventory = OpenMod.Extensions.Games.Abstractions.Items.IHasInventory;

namespace OpenMod.Rust.Players
{
    public class RustPlayer : RustEntity, IPlayer, IComparable<RustPlayer>, IHasInventory, IHasHealth
    {
        public BasePlayer Player { get; }

        public RustPlayer(BasePlayer player) : base(player)
        {
            Player = player;
            EntityInstanceId = player.UserIDString;
            Inventory = new RustPlayerInventory(Player.inventory);
            Asset = new RustPlayerAsset(player);
            // Rust todo: set stance
        }

        public override IEntityAsset Asset { get; }

        public string Stance
        {
            get
            {
                throw new NotImplementedException();
                //Stance is flag base with multiple flags at once
                //Player.modelState.flags & ModelState.Flag.Sprinting
            }
        }

        public int CompareTo(RustPlayer other)
        {
            return ReferenceEquals(this, other) ? 0 : string.Compare(EntityInstanceId, other.EntityInstanceId, StringComparison.Ordinal);
        }

        protected override bool DoTeleport(Vector3 destination, Quaternion rotation)
        {
            if (Player.IsSpectating())
            {
                return false;
            }

            using (PlayerServerFall.Enable(Player))
            {
                // Can't teleport if mounted
                Player.EnsureDismounted();
                Player.SetParent(entity: null, worldPositionStays: true, sendImmediate: true);

                var uvector = destination.ToUnityVector();
                Player.Teleport(uvector);
            }

            return true;
        }

        public IInventory Inventory { get; }

        public IPAddress? Address
        {
            get
            {
                return Player.Connection?.ipaddress == null
                    ? null
                    : IPAddress.Parse(Player.Connection.ipaddress);
            }
        }

        public bool IsAlive
        {
            get
            {
                return Player.IsAlive();
            }
        }

        public double MaxHealth
        {
            get
            {
                return Player.MaxHealth();
            }
        }

        public double Health
        {
            get
            {
                return Player.Health();
            }
        }

        public Task SetHealthAsync(double health)
        {
            async UniTask SetHealthTask()
            {
                await UniTask.SwitchToMainThread();
                Player.SetHealth((float)health);
            }

            return SetHealthTask().AsTask();
        }

        public Task DamageAsync(double amount)
        {
            async UniTask DamageTask()
            {
                await UniTask.SwitchToMainThread();
                Player.Hurt((float) amount);
            }

            return DamageTask().AsTask();
        }

        public Task KillAsync()
        {
            async UniTask KillTask()
            {
                await UniTask.SwitchToMainThread();
                Player.Kill();
            }

            return KillTask().AsTask();
        }
    }
}
