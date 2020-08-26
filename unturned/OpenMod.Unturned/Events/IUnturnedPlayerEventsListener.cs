using SDG.Unturned;

namespace OpenMod.Unturned.Events
{
    internal interface IUnturnedPlayerEventsListener : IUnturnedEventsListener
    {
        void SubscribePlayer(Player player);

        void UnsubscribePlayer(Player player);
    }
}
