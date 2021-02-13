using System;
using OpenMod.Core.Rcon.Tcp;

namespace OpenMod.Unturned.RocketMod.Rcon
{
    public class RocketModRconHost : BaseTcpRconHost<RocketModRconClient>
    {
        public RocketModRconHost(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}