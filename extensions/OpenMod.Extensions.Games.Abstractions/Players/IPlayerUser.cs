using OpenMod.API.Users;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// Represents a player user.
    /// </summary>
    public interface IPlayerUser : IUser
    {
        /// <value>
        /// The player.
        /// </value>
        public IPlayer Player { get; }
    }

    /// <summary>
    /// Represents a player user.
    /// </summary>
    /// <typeparam name="T">The player type.</typeparam>
    public interface IPlayerUser<out T> : IPlayerUser where T : IPlayer
    {
        /// <value>
        /// The player.
        /// </value>
        public new T Player { get; }
    }
}