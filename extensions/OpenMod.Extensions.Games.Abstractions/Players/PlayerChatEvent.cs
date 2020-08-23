using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public class PlayerChatEvent : PlayerEvent, ICancellableEvent
    {
        public string Message { get; }

        public PlayerChatEvent(IPlayer player, string message) : base(player)
        {
            Message = message;
        }

        public bool IsCancelled { get; set; }
    }
}