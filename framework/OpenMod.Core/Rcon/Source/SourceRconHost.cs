using System;
using OpenMod.Core.Rcon.Tcp;

namespace OpenMod.Core.Rcon.Source
{
    public class SourceRconHost : BaseTcpRconHost<SourceRconClient>
    {
        public SourceRconHost(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}