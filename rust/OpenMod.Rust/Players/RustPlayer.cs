using System;
using System.Net;
using System.Numerics;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Rust.Entities;
using OpenMod.Rust.Items;
using OpenMod.UnityEngine.Extensions;
using IHasInventory = OpenMod.Extensions.Games.Abstractions.Items.IHasInventory;

namespace OpenMod.Rust.Players
{
    public class RustPlayer : RustEntity, IPlayer, IComparable<RustPlayer>, IHasInventory
    {
        public BasePlayer Player { get; }

        public RustPlayer(BasePlayer player) : base(player)
        {
            Player = player;
            EntityInstanceId = player.UserIDString;
            Inventory = new RustPlayerInventory(Player.inventory);

            // Rust todo: set stance
        }

        public string Stance
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int CompareTo(RustPlayer other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            return string.Compare(EntityInstanceId, other.EntityInstanceId, StringComparison.Ordinal);
        }

        protected override bool DoTeleport(Vector3 destination, Vector3 rotation)
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

        // todo: get IP of RustPlayer
        public IPAddress? IP => throw new NotImplementedException();
    }
}
