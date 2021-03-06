using OpenMod.API;

namespace OpenMod.Extensions.Games.Abstractions
{
    /// <summary>
    /// Represents common game capabilities.
    /// Can be used to check if the current game supports specific features.
    /// See <see cref="IOpenModHost.HasCapability"/>.
    /// </summary>
    public static class KnownGameCapabilities
    {
        public static readonly string Inventory = "games-inventory";
        public static readonly string Health = "games-health";
        public static readonly string Vehicles = "games-vehicles";
    }
}