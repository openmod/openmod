using OpenMod.Core.Eventing;

namespace OpenMod.Core.Rcon.Events
{
    public abstract class RconClientEvent : Event
    {
        public IRconClient Client { get; }

        protected RconClientEvent(IRconClient client)
        {
            Client = client;
        }
    }
}