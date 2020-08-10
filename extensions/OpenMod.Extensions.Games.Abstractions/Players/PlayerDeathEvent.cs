namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public class PlayerDeathEvent : PlayerEvent
    {
        public PlayerDeathEvent(IPlayer player) : base(player)
        {
        }
    }
}