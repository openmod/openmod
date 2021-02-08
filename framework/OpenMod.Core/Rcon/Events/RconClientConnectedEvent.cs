namespace OpenMod.Core.Rcon.Events
{
    public class RconClientConnectedEvent : RconClientEvent
    {
        public RconClientConnectedEvent(IRconClient client) : base(client)
        {

        }
    }
}