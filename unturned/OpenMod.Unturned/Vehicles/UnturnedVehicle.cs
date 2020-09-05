using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.UnityEngine.Transforms;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace OpenMod.Unturned.Vehicles
{
    public class UnturnedVehicle : IVehicle
    {
        public InteractableVehicle Vehicle { get; }

        public UnturnedVehicle(InteractableVehicle vehicle)
        {
            Vehicle = vehicle;
            Asset = new UnturnedVehicleAsset(vehicle.asset);
            State = new UnturnedVehicleState(vehicle);
            Transform = new UnityTransform(vehicle.transform);
            VehicleInstanceId = vehicle.instanceID.ToString();
            Ownership = new UnturnedVehicleOwnership(vehicle);
        }

        public IVehicleAsset Asset { get; }

        public IVehicleState State { get; }

        public IWorldTransform Transform { get; }

        public string VehicleInstanceId { get; }

        public IEntity Driver
        {
            get
            {
                var driverPassenger = Vehicle.passengers?.ElementAtOrDefault(index: 0);
                if (driverPassenger == null)
                {
                    return null;
                }

                return new UnturnedPlayer(driverPassenger.player.player);
            }
        }

        public IReadOnlyCollection<IEntity> Passengers
        {
            get
            {
                if (Vehicle.passengers == null || Vehicle.passengers.Length == 0)
                {
                    return new List<IEntity>();
                }

                return Vehicle.passengers
                    .Where(d => d?.player != null)
                    .Select(d => new UnturnedPlayer(d.player.player))
                    .ToList();
            }
        }

        public IOwnership Ownership { get; }

        public Task<bool> AddPassengerAsync(IEntity passenger)
        {
            if (!(passenger is UnturnedPlayer))
            {
                return Task.FromException<bool>(
                    new NotSupportedException($"Can not add entity {passenger.GetType()} as passenger"));
            }


            throw new System.NotImplementedException();
        }

        public Task<bool> RemovePassengerAsync(IEntity passenger)
        {
            if (!(passenger is UnturnedPlayer))
            {
                return Task.FromException<bool>(
                    new NotSupportedException($"Can not remove entity {passenger.GetType()} as passenger"));
            }

            throw new System.NotImplementedException();
        }

        public Task DestroyAsync()
        {
            async UniTask DestroyTask()
            {
                await UniTask.SwitchToMainThread();

                // todo may need replication
                Object.Destroy(Vehicle);
            }

            return DestroyTask().AsTask();
        }
    }
}