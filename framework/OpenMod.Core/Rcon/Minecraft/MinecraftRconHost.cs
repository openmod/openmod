using System;
using JetBrains.Annotations;
using OpenMod.Core.Rcon.Tcp;

namespace OpenMod.Core.Rcon.Minecraft
{
    public class MinecraftRconHost : BaseTcpRconHost<MinecraftRconClient>
    {
        public MinecraftRconHost(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}