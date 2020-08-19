using OpenMod.API.Users;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public interface IPlayerUser<out T> : IUser where T : IPlayer
    {
        public T Player { get; }
    }
}