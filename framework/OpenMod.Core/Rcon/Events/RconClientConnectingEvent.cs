using System.Threading.Tasks;
using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Rcon.Events
{
    public class RconClientConnectingEvent : RconClientEvent, ICancellableEvent
    {
        public RconClientConnectingEvent(IRconClient client) : base(client)
        {

        }

        public bool IsCancelled { get; set; }

        public string? RejectionReason { get; set; }

        public async Task RejectAsync(string? reason)
        {
            await Client.DisconnectAsync(reason);
        }
    }
}