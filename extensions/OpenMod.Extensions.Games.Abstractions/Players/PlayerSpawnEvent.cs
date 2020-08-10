namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public class PlayerSpawnEvent : PlayerEvent
    {
        public PlayerSpawnEvent(IPlayer player) : base(player)
        {
        }
    }
}