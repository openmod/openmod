using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Vehicles
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class UnturnedVehicleDirectory : IVehicleDirectory
    {
        public Task<IReadOnlyCollection<IVehicleAsset>> GetVehicleAssetsAsync()
        {
            var items = Assets.find(EAssetType.VEHICLE)
                .Cast<VehicleAsset>()
                .Select(d => new UnturnedVehicleAsset(d))
                .ToList();

            return Task.FromResult<IReadOnlyCollection<IVehicleAsset>>(items);
        }

        public Task<IReadOnlyCollection<IVehicle>> GetVehiclesAsync()
        {
            return Task.FromResult<IReadOnlyCollection<IVehicle>>(VehicleManager.vehicles
                .ConvertAll(v => new UnturnedVehicle(v)));
        }
    }
}