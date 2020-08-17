using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.UnityEngine.Transforms;
using SDG.Unturned;

namespace OpenMod.Unturned.Vehicles
{
    public class UnturnedVehicle : IVehicle
    {
        public InteractableVehicle Vehicle { get; }

        public UnturnedVehicle( InteractableVehicle vehicle)
        {
            Vehicle = vehicle;
            // Asset = new UnturnedVehicleAsset(vehicle.asset);
            // State = new UnturnedVehicleState(vehicle);
            Transform = new UnityTransform(vehicle.transform);
            VehicleInstanceId = vehicle.instanceID.ToString();
        }

        public IVehicleAsset Asset { get; }

        public IVehicleState State { get; }

        public IWorldTransform Transform { get; }

        public string VehicleInstanceId { get; }

        public IEntity Driver { get; }

        public IReadOnlyCollection<IEntity> Passengers { get; }

        public Task<bool> AddPassengerAsync(IEntity passenger)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> RemovePassengerAsync(IEntity passenger)
        {
            throw new System.NotImplementedException();
        }


        public Task DestroyAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}