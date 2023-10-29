using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using SDG.Unturned;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Vehicles
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class UnturnedVehicleDirectory : IVehicleDirectory
    {
        public Task<IReadOnlyCollection<IVehicleAsset>> GetVehicleAssetsAsync()
        {
            var assets = new List<VehicleAsset>();
            Assets.find(assets);

            var items = assets
                .ConvertAll(d => new UnturnedVehicleAsset(d));

            return Task.FromResult<IReadOnlyCollection<IVehicleAsset>>(items);
        }

        public Task<IReadOnlyCollection<IVehicle>> GetVehiclesAsync()
        {
            return Task.FromResult<IReadOnlyCollection<IVehicle>>(VehicleManager.vehicles
                .ConvertAll(v => new UnturnedVehicle(v)));
        }
    }
}