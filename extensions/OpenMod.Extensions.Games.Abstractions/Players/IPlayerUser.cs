using OpenMod.API.Users;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public interface IPlayerUser : IUser
    {
        public IPlayer Player { get; }
    }

    public interface IPlayerUser<out T> : IPlayerUser where T : IPlayer
    {
        public new T Player { get; }
    }
}