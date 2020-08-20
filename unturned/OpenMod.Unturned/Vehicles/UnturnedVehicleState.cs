using OpenMod.Extensions.Games.Abstractions.Vehicles;
using SDG.Unturned;

namespace OpenMod.Unturned.Vehicles
{
    public class UnturnedVehicleState : IVehicleState
    {
        private readonly InteractableVehicle m_Vehicle;

        public UnturnedVehicleState(InteractableVehicle vehicle)
        {
            m_Vehicle = vehicle;
        }

        public byte[] StateData
        {
            get
            {
                // todo: replicate state from VehicleManager.save();

                return null;
            }
        }
    }
}