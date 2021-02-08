namespace OpenMod.Core.Rcon.Events
{
    public class RconClientDisconnectedEvent : RconClientEvent
    {
        public ConnectionCloseReason ConnectionCloseReason { get; }

        public RconClientDisconnectedEvent(IRconClient client, ConnectionCloseReason connectionCloseReason) : base(client)
        {
            ConnectionCloseReason = connectionCloseReason;
        }
    }
}