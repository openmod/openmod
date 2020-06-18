using System.Net.Sockets;
using System.Linq;
using System;

namespace Rocket.Core.RCON
{
    public class RCONConnection
    {
        public TcpClient Client;
        public bool Authenticated;
        public bool Interactive;
        public int InstanceID { get; private set; }
        public DateTime ConnectedTime { get; private set; }

        public RCONConnection(TcpClient client, int instance)
        {
            this.Client = client;
            Authenticated = false;
            Interactive = true;
            InstanceID = instance;
            ConnectedTime = DateTime.Now;
        }

        public void Send(string command, bool nonewline = false)
        {
            if (Interactive)
            {
                if (nonewline == true)
                    RCONServer.Send(Client, command);
                else
                    RCONServer.Send(Client, command + (!command.Contains('\n') ? "\r\n" : ""));
                return;
            }
        }

        public string Read()
        {
            return RCONServer.Read(Client, Authenticated);
        }

        public void Close()
        {
            if (Client.Client.Connected)
                Client.Close();
        }

        public string Address { get { return Client.Client.Connected ? Client.Client.RemoteEndPoint.ToString() : "?"; } }
    }

}