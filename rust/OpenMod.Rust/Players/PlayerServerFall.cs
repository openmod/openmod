using System;
using OpenMod.Core.Helpers;

namespace OpenMod.Rust.Players
{
    public static class PlayerServerFall
    {
        public static IDisposable Enable(BasePlayer player)
        {
            player.EnableServerFall(wantsOn: true);
            return new DisposeAction(() =>
            {
                player.EnableServerFall(wantsOn: false);
            });
        }
    }
}