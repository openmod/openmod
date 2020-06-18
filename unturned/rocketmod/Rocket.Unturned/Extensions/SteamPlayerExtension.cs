using Rocket.Unturned.Player;

namespace Rocket.Unturned.Extensions
{
    public static class SteamPlayerExtension
    {
        public static UnturnedPlayer ToUnturnedPlayer(this SDG.Unturned.SteamPlayer player)
        {
            return UnturnedPlayer.FromSteamPlayer(player);
        }
    }
}
