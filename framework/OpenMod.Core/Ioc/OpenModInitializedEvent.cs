using Microsoft.Extensions.Hosting;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Ioc
{
    public sealed class OpenModInitializedEvent : Event
    {
        public IHost Host { get; }

        public OpenModInitializedEvent(IHost host)
        {
            Host = host;
        }
    }
}