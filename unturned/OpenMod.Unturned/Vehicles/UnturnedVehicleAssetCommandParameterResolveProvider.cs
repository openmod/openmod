using OpenMod.API.Commands;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using SDG.Unturned;
using System;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Vehicles
{
    public class UnturnedVehicleAssetCommandParameterResolveProvider : ICommandParameterResolveProvider
    {
        private readonly IVehicleDirectory m_VehicleDirectory;

        public UnturnedVehicleAssetCommandParameterResolveProvider(IVehicleDirectory vehicleDirectory)
        {
            m_VehicleDirectory = vehicleDirectory;
        }

        public bool Supports(Type type)
        {
            return type == typeof(UnturnedVehicleAsset) || type == typeof(IVehicleAsset) || type == typeof(VehicleAsset);
        }

        public async Task<object?> ResolveAsync(Type type, string input)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!Supports(type))
            {
                throw new ArgumentException("The given type is not supported", nameof(type));
            }

            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            var vehicleAsset = await m_VehicleDirectory.FindByIdAsync(input) ??
                               await m_VehicleDirectory.FindByNameAsync(input, false);

            if (type == typeof(IVehicleAsset))
            {
                return vehicleAsset;
            }

            var unturnedVehicleAsset = vehicleAsset as UnturnedVehicleAsset;

            if (type == typeof(UnturnedVehicleAsset))
            {
                return unturnedVehicleAsset;
            }

            if (type == typeof(VehicleAsset))
            {
                return unturnedVehicleAsset?.VehicleAsset;
            }

            throw new InvalidOperationException($"Unable to return type {type}");
        }
    }
}
