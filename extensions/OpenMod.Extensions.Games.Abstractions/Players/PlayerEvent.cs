using OpenMod.Core.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public abstract class PlayerEvent : Event
    {
        protected PlayerEvent(IPlayer player)
        {
            Player = player;
        }

        public IPlayer Player { get; }
    }
}