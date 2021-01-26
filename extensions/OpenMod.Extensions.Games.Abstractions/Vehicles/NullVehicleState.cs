namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    /// <summary>
    /// The null vehicle state for vehicles that do not have a state.
    /// </summary>
    public class NullVehicleState : IVehicleState
    {
        public byte[]? StateData { get; } = null;
    }
}