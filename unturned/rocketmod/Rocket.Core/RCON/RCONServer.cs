using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Rocket.API;
using Logger = Rocket.Core.Logging.Logger;

namespace Rocket.Core.RCON
{
    public class RCONServer : MonoBehaviour
    {
        private static List<RCONConnection> clients = new List<RCONConnection>();
        public static List<RCONConnection> Clients { get { return clients; } }
        private TcpListener listener;
        private bool exiting = false;
        private Thread waitingThread;
        private static int instanceID = 0;

        private static Queue<string> commands = new Queue<string>();

        public void Awake()
        {
            listener = new TcpListener(IPAddress.Any, R.Settings.Instance.RCON.Port);
            listener.Start();

            // Logger.Log("Waiting for new connection...");

            waitingThread = new Thread(() =>
            {
                while (!exiting)
                {
                    instanceID++;
                    RCONConnection newclient = new RCONConnection(listener.AcceptTcpClient(), instanceID);
                    clients.Add(newclient);
                    newclient.Send("RocketRcon v" + Assembly.GetExecutingAssembly().GetName().Version + "\r\n");
                    ThreadPool.QueueUserWorkItem(handleConnection, newclient);
                }
            });
            waitingThread.Start();
        }

        private static void handleConnection(object obj)
        {
            try
            {
                RCONConnection newclient = (RCONConnection)obj;
                string command = "";

                int nonAuthCommandCount = 0;
                bool maxClientsReached = false;
                if (R.Settings.Instance.RCON.EnableMaxGlobalConnections)
                {
                    if (clients.Count > R.Settings.Instance.RCON.MaxGlobalConnections)
                    {
                        maxClientsReached = true;
                        newclient.Send("Error: Too many clients connected to RCON, not accepting connection!\r\n");
                        Logger.LogWarning("Maximum global RCON connections has been reached.");
                    }
                }
                if (R.Settings.Instance.RCON.EnableMaxLocalConnections && !maxClientsReached)
                {
                    int currentLocalCount = 0;
                    for (int i = 0; i < clients.Count; i++)
                    {
                        if (newclient.Client.Client.Connected && clients[i].Client.Client.Connected)
                            if (((IPEndPoint)newclient.Client.Client.RemoteEndPoint).Address.Equals(((IPEndPoint)clients[i].Client.Client.RemoteEndPoint).Address))
                            {
                                currentLocalCount++;
                                if (currentLocalCount > R.Settings.Instance.RCON.MaxLocalConnections)
                                {
                                    maxClientsReached = true;
                                    newclient.Send("Error: Too many clients connected from your address, not accepting connection!\r\n");
                                    Logger.LogWarning("Maximum Local RCON connections has been reached for address: " + newclient.Address + ".");
                                    break;
                                }
                            }
                    }
                }

                while (newclient.Client.Client.Connected && !maxClientsReached)
                {
                    Thread.Sleep(100);
                    command = newclient.Read();
                    if (command == "") break;
                    if (!newclient.Authenticated)
                    {
                        nonAuthCommandCount++;
                        if (nonAuthCommandCount > 4)
                        {
                            newclient.Send("Error: Too many commands sent before Authentication!\r\n");
                            Logger.LogWarning("Client has sent too many commands before Authentication!");
                            break;
                        }
                    }
                    command = command.Trim('\n', '\r', ' ', '\0');
                    if (command == "quit") break;
                    if (command == "ia")
                    {
                        //newclient.Send("Toggled interactive mode");
                        newclient.Interactive = !newclient.Interactive;
                    }
                    if (command == "") continue;
                    if (command == "login")
                    {
                        if (newclient.Authenticated)
                            newclient.Send("Notice: You are already logged in!\r\n");
                        else
                            newclient.Send("Syntax: login <password>\r\n");
                        continue;
                    }
                    if (command.Split(' ').Length > 1 && command.Split(' ')[0] == "login")
                    {
                        if (newclient.Authenticated)
                        {
                            newclient.Send("Notice: You are already logged in!\r\n");
                            continue;
                        }
                        else
                        {

                            if (command.Split(' ')[1] == R.Settings.Instance.RCON.Password)
                            {
                                newclient.Authenticated = true;
                                //newclient.Send("Success: You have logged in!\r\n");
                                //Logger.Log("Client has logged in!");
                                continue;
                            }
                            else
                            {
                                newclient.Send("Error: Invalid password!\r\n");
                                Logger.LogWarning("Client has failed to log in.");
                                break;
                            }
                        }
                    }

                    if (command == "set")
                    {
                        newclient.Send("Syntax: set [option] [value]");
                        continue;
                    }
                    if (!newclient.Authenticated)
                    {
                        newclient.Send("Error: You have not logged in yet! Login with syntax: login <password>\r\n");
                        continue;
                    }
                    if (command != "ia")
                        Logger.Log("Client ID: " + newclient.InstanceID + " has executed command \"" + command + "\"");

                    lock (commands)
                    {
                        commands.Enqueue(command);
                    }
                    command = "";
                }


                clients.Remove(newclient);
                newclient.Send("Good bye!");
                Thread.Sleep(1500);
                Logger.Log("Client ID: " + newclient.InstanceID + " has disconnected! (IP: " + newclient.Address + ")");
                newclient.Close();

            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
            }
        }

        private void FixedUpdate()
        {
            lock (commands)
            {
                while(commands.Count!=0)
                    R.Commands.Execute(new ConsolePlayer(), commands.Dequeue());
            }
        }

        public static void Broadcast(string message)
        {
            foreach (RCONConnection client in clients)
            {
                if (client.Authenticated)
                    client.Send(message);
            }
        }

        private void OnDestroy()
        {
            exiting = true;
            // Force all connected RCON clients to disconnect from the server on shutdown. The server will get stuck in the shutdown process until all clients disconnect.
            List<RCONConnection> connections = new List<RCONConnection>();
            connections.AddRange(clients);
            foreach (RCONConnection client in connections)
            {
                client.Close();
            }
            clients.Clear();
            waitingThread.Abort();
            listener.Stop();
        }

        public static string Read(TcpClient client, bool auth)
        {
            byte[] _data = new byte[1];
            List<byte> dataArray = new List<byte>();
            string data = "";
            int loopCount = 0;
            int skipCount = 0;

            try
            {
                if (client.Client.Connected)
                {
                    NetworkStream _stream = client.GetStream();
                    while (true)
                    {
                        loopCount++;
                        if (loopCount > 2048)
                            break;
                        int k = _stream.Read(_data, 0, 1);
                        if (k == 0)
                            return "";
                        byte b = _data[0];
                        // Ignore Putty connection Preamble.
                        if (!auth && b == 0xFF && skipCount <= 0)
                        {
                            skipCount = 2;
                            continue;
                        }
                        else if (!auth && skipCount > 0)
                        {
                            skipCount--;
                            continue;
                        }
                        dataArray.Add(b);
                        //Logger.Log(BitConverter.ToString(_data) + ":" + Encoding.UTF8.GetString(_data, 0, 1));
                        // break on \r and \n.
                        if (b == 0x0D || b == 0x0A)
                            break;
                    }
                    // Convert byte array into UTF8 string.
                    data = Encoding.UTF8.GetString(dataArray.ToArray(), 0, dataArray.Count);
                }
            }
            catch (Exception ex)
            {
                // "if" disables error message on Read for lost or force closed connections(ie, kicked by command.).
                if (client.Client.Connected)
                    Logger.LogException(ex);
                return "";
            }
            return data;
        }

        public static void Send(TcpClient client, string text)
        {
            byte[] data = new UTF8Encoding().GetBytes(text);
            try
            {
                if (client.Client.Connected)
                    client.GetStream().Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}