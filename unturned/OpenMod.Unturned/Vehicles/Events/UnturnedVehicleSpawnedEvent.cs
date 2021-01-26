namespace OpenMod.Unturned.Vehicles.Events
{
    /// <summary>
    /// The event that is triggered when a vehicle has spawned.
    /// </summary>
    public class UnturnedVehicleSpawnedEvent : UnturnedVehicleEvent
    {
        public UnturnedVehicleSpawnedEvent(UnturnedVehicle vehicle) : base(vehicle)
        {

        }
    }
}
