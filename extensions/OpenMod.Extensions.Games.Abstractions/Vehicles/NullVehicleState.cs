namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    public class NullVehicleState : IVehicleState
    {
        public byte[] StateData { get; } = null;
    }
}