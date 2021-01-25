using OpenMod.Core.Users;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents common entity types. Support depends on the game.
    /// </summary>
    public static class KnownEntityTypes
    {
        public const string Player = KnownActorTypes.Player;
        public const string Zombie = "zombie";
        public const string Npc = "npc";
        public const string Vehicle = "vehicle";
    }
}