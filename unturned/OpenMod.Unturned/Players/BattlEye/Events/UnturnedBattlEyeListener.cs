using System;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players.BattlEye.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.BattleEye.Events;
internal class UnturnedBattlEyeListener : UnturnedEventsListener
{
    public UnturnedBattlEyeListener(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override void Subscribe()
    {
        Provider.onBattlEyeKick += Provider_onBattlEyeKick;
    }

    public override void Unsubscribe()
    {
        Provider.onBattlEyeKick -= Provider_onBattlEyeKick;
    }

    private void Provider_onBattlEyeKick(SteamPlayer client, string reason)
    {
        var player = GetUnturnedPlayer(client)!;
        var @event = new UnturnedBattlEyeKickedEvent(player, reason);

        Emit(@event);
    }
}
