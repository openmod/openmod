namespace OpenMod.Core.Rcon.Events
{
    public class RconClientAuthenticatingEvent : RconClientEvent
    {
        public string? Username { get; }
        public string? Password { get; }

        public RconClientAuthenticatingEvent(IRconClient client, string? username, string? password) : base(client)
        {
            Username = username;
            Password = password;
        }

        public bool IsAuthenticated { get; set; }
    }
}